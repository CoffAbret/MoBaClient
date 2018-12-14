using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    //UDP连接
    private Socket m_Client;
    //接收数据时远程主机的信息
    private IPEndPoint m_RemoteEP;
    //网络IP地址
    private IPAddress m_Address;
    //接收消息数据
    private List<CReadPacket> m_ReceivePacketList;
    //接收数据长度
    private int length = 1024;
    #endregion
    //网络数据派发
    public delegate void MessageHandle(CReadPacket msg);
    public event MessageHandle OnMessage;
    public AsyncTcpClient() { }
    public AsyncTcpClient(string ip, int port)
    {
        m_ReceivePacketList = new List<CReadPacket>();
        m_Address = IPAddress.Parse(ip);
        IPEndPoint point = new IPEndPoint(m_Address, port);
        m_Client = new Socket(m_Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        m_Client.BeginConnect(point, AsyncConnectData, m_Client);
    }

    /// <summary>
    /// 开始连接回调
    /// </summary>
    /// <param name="result"></param>
    private void AsyncConnectData(IAsyncResult result)
    {
        if (result.IsCompleted)
            AsyncBeginReceive();
    }

    /// <summary>
    /// 开始监听
    /// </summary>
    private void AsyncBeginReceive()
    {
        if (m_Client.Connected)
        {
            AsyncSocketState newState = new AsyncSocketState();
            newState.m_ReceiveBuffer = new byte[length];
            m_Client.BeginReceive(newState.m_ReceiveBuffer, 0, newState.m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallback, newState);
        }
    }
    /// <summary>
    /// 开始监听回调
    /// </summary>
    /// <param name="result"></param>
    private void ReceiveCallback(IAsyncResult result)
    {
        if (result.IsCompleted && m_Client.Connected)
        {
            try
            {
                AsyncSocketState state = (AsyncSocketState)result.AsyncState;
                int length = m_Client.EndReceive(result);
                if (length < 1 || state == null)
                    return;
                CReadPacket readPacket = new CReadPacket(state.m_ReceiveBuffer, length);
                readPacket.ReadData();
                m_ReceivePacketList.Add(readPacket);
                state.m_ReceiveBuffer = null;
                state = null;
            }
            catch (Exception ex)
            {
                GameData.m_GameManager.m_LogMessage.text += string.Format("{0},", ex.Message);
            }
            finally
            {
                AsyncBeginReceive();
            }
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data"></param>
    public void AsyncSendData(CWritePacket data)
    {
        m_Client.BeginSend(data.GetPacketByte(), 0, data.GetPacketByte().Length, SocketFlags.None, SendCallback, null);
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

/// <summary>
/// 接收数据类
/// </summary>
public class AsyncSocketState
{
    //接收数据数组
    public byte[] m_ReceiveBuffer;
}
