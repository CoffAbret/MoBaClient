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
        CWritePacket writePacket = new CWritePacket(NetProtocol.CONNECT);
        AsyncSendData(writePacket);
        AsyncReceiveData();
    }

    /// <summary>
    /// 开始监听
    /// </summary>
    private void AsyncReceiveData()
    {
        try
        {
            m_Client.BeginReceive(ReceiveCallback, m_Client);
        }
        catch (Exception ex)
        {
            GameData.m_GameManager.m_LogMessage.text += string.Format("{0},", ex.Message);
        }
    }

    /// <summary>
    /// 开始监听回调
    /// </summary>
    /// <param name="result"></param>
    private void ReceiveCallback(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            try
            {
                byte[] data = m_Client.EndReceive(result, ref m_RemoteEP);
                if (data == null || data.Length < 1)
                    return;
                CReadPacket readPacket = new CReadPacket(data, data.Length);
                readPacket.ReadData();
                m_ReceivePacketList.Add(readPacket);
            }
            catch (Exception ex)
            {
                GameData.m_GameManager.m_LogMessage.text += string.Format("{0},", ex.Message);
            }
            finally
            {
                m_Client.BeginReceive(ReceiveCallback, null);
            }
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendData(CWritePacket data)
    {
        m_Client.BeginSend(data.GetPacketByte(), data.GetPacketByte().Length, new AsyncCallback(SendCallback), null);
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
        m_Client.BeginSend(writePacket.GetPacketByte(), writePacket.GetPacketByte().Length, new AsyncCallback(SendCallback), null);
    }

    /// <summary>
    /// 发送数据回调
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
                GameData.m_GameManager.m_LogMessage.text += string.Format("{0},", ex.Message);
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
