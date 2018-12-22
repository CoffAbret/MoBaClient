using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Tcp网络连接
/// </summary>
public class AsyncTcpClient
{
    #region 私有变量
    //Tcp连接
    private TcpClient m_Client = null;
    //数据存储对象
    private MemoryStream m_MemoryStream = null;
    //数据读取对象
    private BinaryReader m_BinaryReader = null;
    //最大接收数据长度
    private int m_MaxLength = 1024;
    //接收数据字节数组
    private byte[] m_Buffer;
    //接收消息数据
    private List<CReadPacket> m_ReceivePacketList;
    #endregion
    //网络数据派发
    public delegate void MessageHandle(CReadPacket msg);
    public event MessageHandle OnMessage;
    public AsyncTcpClient() { }
    /// <summary>
    /// 初始化网络连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public AsyncTcpClient(string ip, int port)
    {
        m_ReceivePacketList = new List<CReadPacket>();
        m_MemoryStream = new MemoryStream();
        m_BinaryReader = new BinaryReader(m_MemoryStream);
        IPAddress[] address = Dns.GetHostAddresses(ip);
        if (address == null || address.Length < 0)
            return;
        if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            m_Client = new TcpClient(AddressFamily.InterNetworkV6);
        if (address[0].AddressFamily == AddressFamily.InterNetwork)
            m_Client = new TcpClient(AddressFamily.InterNetwork);
        if (m_Client == null)
            return;
        m_Client.SendTimeout = 1000;
        m_Client.ReceiveTimeout = 1000;
        m_Client.NoDelay = true;
        m_Client.BeginConnect(ip, port, new AsyncCallback(OnConnect), null);
    }

    /// <summary>
    /// 开始连接回调
    /// </summary>
    /// <param name="result"></param>
    private void OnConnect(IAsyncResult result)
    {

        if (m_Client == null || !m_Client.Connected)
            return;
        m_Buffer = new byte[m_MaxLength];
        m_Client.GetStream().BeginRead(m_Buffer, 0, m_MaxLength, new AsyncCallback(OnRead), null);
    }

    /// <summary>
    /// 开始接收数据回调
    /// </summary>
    /// <param name="asr"></param>
    private void OnRead(IAsyncResult result)
    {
        if (m_Client == null || !m_Client.Connected)
            return;
        int readLength;
        try
        {
            lock (m_Client.GetStream())
            {
                readLength = m_Client.GetStream().EndRead(result);
            }
            m_MemoryStream.Seek(0, SeekOrigin.End);
            m_MemoryStream.Write(m_Buffer, 0, readLength);
            m_MemoryStream.Seek(0, SeekOrigin.Begin);
            //粘包处理
            while ((m_MemoryStream.Length - m_MemoryStream.Position) > sizeof(int))
            {
                int messageLength = IPAddress.NetworkToHostOrder(m_BinaryReader.ReadInt32());
                m_MemoryStream.Position = m_MemoryStream.Position - sizeof(int);
                if (m_MemoryStream.Length - m_MemoryStream.Position >= messageLength)
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter write = new BinaryWriter(stream);
                    write.Write(m_BinaryReader.ReadBytes(messageLength));
                    stream.Seek(0, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] message = reader.ReadBytes((int)(stream.Length - stream.Position));
                    CReadPacket readPacket = new CReadPacket(message, message.Length);
                    readPacket.ReadData();
                    m_ReceivePacketList.Add(readPacket);
                    reader.Close();
                    reader = null;
                    write.Close();
                    write = null;
                    stream.Close();
                    stream = null;
                }
                else
                {
                    m_MemoryStream.Position = m_MemoryStream.Position - sizeof(int);
                }

            }
            //剩余数据写入数据存储对象
            byte[] surplusByte = m_BinaryReader.ReadBytes((int)(m_MemoryStream.Length - m_MemoryStream.Position));
            m_MemoryStream.SetLength(0);
            m_MemoryStream.Write(surplusByte, 0, surplusByte.Length);
            lock (m_Client.GetStream())
            {
                Array.Clear(m_Buffer, 0, readLength);
                m_Client.GetStream().BeginRead(m_Buffer, 0, m_MaxLength, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendData(CWritePacket data)
    {
        if (m_Client == null || !m_Client.Connected)
            return;
        m_Client.GetStream().BeginWrite(data.GetPacketByte(), 0, data.GetPacketByte().Length, new AsyncCallback(OnWrite), null);
    }

    /// <summary>
    /// 发送ping包
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendPing()
    {
        IDictionary<string, object> packet = new Dictionary<string, object>();
        packet.Add("msgid", NetProtocol.PING);
        CWritePacket writePacket = new CWritePacket(NetProtocol.PING);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        m_Client.GetStream().BeginWrite(writePacket.GetPacketByte(), 0, writePacket.GetPacketByte().Length, new AsyncCallback(OnWrite), null);
    }

    /// <summary>
    /// 发送数据回调
    /// </summary>
    /// <param name="result"></param>
    private void OnWrite(IAsyncResult result)
    {
        if (m_Client == null || !m_Client.Connected)
            return;
        try
        {
            m_Client.GetStream().EndWrite(result);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 网络断开
    /// </summary>
    public void Disconnect()
    {
        if (m_Client == null)
            return;
        m_MemoryStream.Close();
        m_BinaryReader.Close();
        m_Client.Close();
        m_MemoryStream = null;
        m_BinaryReader = null;
        m_Client = null;
        m_ReceivePacketList.Clear();
        m_ReceivePacketList = null;
    }

    /// <summary>
    /// 每帧处理网络数据
    /// </summary>
    public void UpdateNet()
    {
        if (m_ReceivePacketList == null)
            return;
        while (m_ReceivePacketList.Count > 0)
        {
            OnMessage(m_ReceivePacketList[0]);
            m_ReceivePacketList.RemoveAt(0);
        }
    }
}
