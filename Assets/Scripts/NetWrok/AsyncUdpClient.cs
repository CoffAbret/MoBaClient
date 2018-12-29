using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Udp网络连接
/// </summary>
public class AsyncUdpClient
{
    #region 私有变量
    //UDP连接
    private UdpClient m_Client;
    //接收数据时远程主机的信息
    private IPEndPoint m_RemoteEP;
    //网络IP地址
    private IPAddress address;
    //网络地址
    private IPEndPoint point;
    //接收消息数据
    private List<CReadPacket> m_ReceivePacketList;
    #endregion
    //网络数据派发
    public delegate void MessageHandle(CReadPacket msg);
    public event MessageHandle OnMessage;
    public AsyncUdpClient() { }
    public AsyncUdpClient(string ip, int port)
    {
        m_ReceivePacketList = new List<CReadPacket>();
        address = IPAddress.Parse(ip);
        point = new IPEndPoint(address, port);
        m_Client = new UdpClient();
        m_Client.Connect(point);
        AsyncReceiveData();
    }

    /// <summary>
    /// 开始监听
    /// </summary>
    private void AsyncReceiveData()
    {
        try
        {
            if (m_Client == null)
                return;
            m_Client.BeginReceive(ReceiveCallback, m_Client);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 开始监听回调
    /// </summary>
    /// <param name="result"></param>
    private void ReceiveCallback(IAsyncResult result)
    {
        if (m_Client == null || !result.IsCompleted)
            return;
        try
        {
            byte[] data = m_Client.EndReceive(result, ref m_RemoteEP);
            if (data == null || data.Length < 1)
                return;
            IoBuffer Io = new IoBuffer(data);
            while (Io.RemainingBytes() > KCP.IKCP_OVERHEAD)
            {
                KcpPacket seg = new KcpPacket();
                seg.m_Cmd = Io.ReadByte();
                seg.m_Win = Io.ReadByte();
                seg.m_Sn = Io.ReadInt();
                seg.m_NextSn = Io.ReadInt();
                seg.m_Ts = Io.ReadLong();
                int len = Io.ReadInt();
                if (len > Io.RemainingBytes())
                {
                    // 如果消息内容不够，则重置，相当于不读取size
                    Io.Position = 0;
                }
                else
                {
                    byte[] bytes = Io.ReadBytes(len);
                    seg.m_Data = bytes;
                    GameData.m_GameManager.m_NetManager.m_KCPClient.recv(seg);
                }
            }
            //CReadPacket readPacket = new CReadPacket(data, data.Length);
            //readPacket.ReadData();
            //GameData.m_GameManager.m_NetManager.m_KCPClient.recv
            //m_ReceivePacketList.Add(readPacket);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        finally
        {
            m_Client.BeginReceive(ReceiveCallback, null);
        }
    }

    /// <summary>
    /// 开始回调
    /// </summary>
    /// <param name="result"></param>
    public void Receive(CReadPacket packet)
    {
        m_ReceivePacketList.Add(packet);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendData(CWritePacket data)
    {
        if (m_Client == null)
            return;
        if (GameData.m_GameManager.m_NetManager.m_KCPClient == null)
            return;
        GameData.m_GameManager.m_NetManager.m_KCPClient.send(data);
        //m_Client.BeginSend(data.GetPacketByte(), data.GetPacketByte().Length, new AsyncCallback(SendCallback), null);
    }


    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendKcpData(KcpPacket kcpData)
    {
        if (m_Client == null)
            return;
        int dataLen = 0;
        IoBuffer buffer = new IoBuffer();
        byte[] bytes = kcpData.m_Data;
        dataLen = bytes == null ? 0 : bytes.Length;
        buffer.Position = 0;
        buffer.WriteByte(kcpData.m_Cmd);
        buffer.WriteByte(kcpData.m_Win);
        buffer.WriteInt(kcpData.m_Sn);
        buffer.WriteInt(kcpData.m_NextSn);
        buffer.WriteLong(kcpData.m_Ts);
        buffer.WriteInt(dataLen);
        if (dataLen > 0)
            buffer.WriteBytes(bytes);
        UnityEngine.Debug.LogError(string.Format("发送：m_Sn:{0},m_Ts:{1}", kcpData.m_Sn, kcpData.m_Ts));
        //GameData.m_GameManager.m_LogMessage.text += string.Format("m_Sn:{0},m_Ts:{1}", kcpData.m_Sn, kcpData.m_Ts);
        m_Client.BeginSend(buffer.ToBytes(), buffer.ToBytes().Length, new AsyncCallback(SendCallback), null);
    }

    /// <summary>
    /// 发送ping包
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendPing()
    {
        //IDictionary<string, object> packet = new Dictionary<string, object>();
        //packet.Add("msgid", NetProtocol.PING);
        //CWritePacket writePacket = new CWritePacket(NetProtocol.PING);
        //StringBuilder builder = Jsontext.WriteData(packet);
        //string json_Str = builder.ToString();
        //writePacket.WriteString(json_Str);
        //m_Client.BeginSend(writePacket.GetPacketByte(), writePacket.GetPacketByte().Length, new AsyncCallback(SendCallback), null);
    }

    /// <summary>
    /// 发送数据回调
    /// </summary>
    /// <param name="result"></param>
    private void SendCallback(IAsyncResult result)
    {
        if (m_Client == null || !result.IsCompleted)
            return;
        try
        {
            m_Client.EndSend(result);
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
        m_Client.Close();
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
