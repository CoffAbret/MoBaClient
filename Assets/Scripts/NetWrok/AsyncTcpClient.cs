using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// 网络连接
/// </summary>
public class AsyncTcpClient
{
    #region 私有变量
    //UDP连接
    private Socket m_Client;
    //接收数据时远程主机的信息
    private IPEndPoint m_RemoteEP;
    //网络IP地址
    private IPAddress m_Address;
    //接收消息数据
    private List<CReadPacket> m_ReceivePacketList;
    //接收数据
    private byte[] m_ReceiveBuffer;
    #endregion
    //网络数据派发
    public delegate void MessageHandle(CReadPacket msg);
    public event MessageHandle OnMessage;
    public AsyncTcpClient() { }
    public AsyncTcpClient(string ip, int port)
    {
        m_ReceiveBuffer = new byte[1024];
        m_ReceivePacketList = new List<CReadPacket>();
        m_Address = IPAddress.Parse(ip);
        IPEndPoint point = new IPEndPoint(m_Address, port);
        m_Client = new Socket(m_Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        m_Client.Bind(point);
        m_Client.BeginConnect(point, AsyncConnectData, null);
    }

    private void AsyncConnectData(IAsyncResult result)
    {
        if (m_Client.Connected)
            m_Client.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
    }
    /// <summary>
    /// 接收数据的回调函数
    /// </summary>
    /// <param name="result"></param>
    private void ReceiveCallback(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            try
            {
                if (m_ReceiveBuffer.Length < 1)
                    return;
                CReadPacket readPacket = new CReadPacket(m_ReceiveBuffer, m_ReceiveBuffer.Length);
                readPacket.ReadData();
                OnMessage(readPacket);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_Client.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
        }
    }

    /// <summary>
    /// 异步发送数据
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendData(CWritePacket data)
    {
        m_Client.BeginSend(data.GetPacketByte(), 0, data.GetPacketByte().Length, SocketFlags.None, SendCallback, null);
    }

    /// <summary>
    /// Ping包
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
        m_Client.BeginSend(writePacket.GetPacketByte(), 0, writePacket.GetPacketByte().Length, SocketFlags.None, SendCallback, null);
    }

    /// <summary>
    /// 发送数据后的回调函数
    /// </summary>
    /// <param name="result"></param>
    private void SendCallback(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            try
            {
                m_Client.EndSend(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 网络断开
    /// </summary>
    public void Disconnect()
    {
        if (m_Client == null)
            return;
        m_Client.Close();
        m_Client = null;
        m_ReceivePacketList.Clear();
        m_ReceivePacketList = null;
    }
}
