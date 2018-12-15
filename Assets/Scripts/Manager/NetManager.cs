using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

/// <summary>
/// 网络连接管理
/// </summary>
public class NetManager
{
    //Tcp网络连接
    public AsyncTcpClient m_TcpClient;
    //Udp网络连接
    public AsyncUdpClient m_UdpClient;

    /// <summary>
    /// 初始化Tcp网络连接
    /// </summary>
    public void InitTcpClient()
    {
        m_TcpClient = new AsyncTcpClient(GameData.m_IP, GameData.m_Port);
        m_TcpClient.OnMessage += OnTcpMessage;
    }

    /// <summary>
    /// 初始化Udp网络连接
    /// </summary>
    public void InitUdpClient()
    {
        m_UdpClient = new AsyncUdpClient(GameData.m_UdpIP, GameData.m_UdpPort);
        m_UdpClient.OnMessage += OnUdpMessage;
    }

    /// <summary>
    /// 每帧处理Tcp网络数据
    /// </summary>
    public void UpdateTcpNet()
    {
        m_TcpClient.UpdateNet();
    }

    /// <summary>
    /// 每帧处理Udp网络数据
    /// </summary>
    public void UpdateNet()
    {
        if (m_UdpClient == null)
            return;
        m_UdpClient.UpdateNet();
        GameData.m_PingTime += GameData.m_FixFrameLen;
        if (GameData.m_PingTime >= Fix64.FromRaw(10000))
        {
            m_UdpClient.AsyncSendPing();
            GameData.m_PingTime = Fix64.Zero;
        }
    }

    /// <summary>
    /// 发送网络数据
    /// </summary>
    /// <param name="buffer"></param>
    public void Send(CWritePacket buffer)
    {
        m_TcpClient.AsyncSendData(buffer);
    }

    /// <summary>
    /// 发送网络数据
    /// </summary>
    /// <param name="buffer"></param>
    public void SendUdp(CWritePacket buffer)
    {
        m_UdpClient.AsyncSendData(buffer);
    }

    /// <summary>
    /// 解析网络数据
    /// </summary>
    /// <param name="buffer"></param>
    public void OnTcpMessage(CReadPacket buffer)
    {
        if (buffer == null)
            return;
        uint protocol = buffer.GetMessageID();
        Dictionary<string, object> data = buffer.data;
        switch (protocol)
        {
            case NetProtocol.LOGIN_RET:
                GameData.m_GameManager.LoginGame(data);
                break;
            case NetProtocol.MATCH_RET:
                GameData.m_GameManager.MatchGame(data);
                break;
            case NetProtocol.MATCH_SUCCESS_RET:
                GameData.m_GameManager.MatchGameSuccess(data);
                break;
            case NetProtocol.MATCH_JOIN_ROOM_RET:
                GameData.m_GameManager.JoinMatchRoom(data);
                break;
            case NetProtocol.MATCH_HERO_ROOM_RET:
                GameData.m_GameManager.JoinMatchHeroRoom(data);
                break;
            case NetProtocol.MATCH_JOIN_ROOM_IN_POSTION:
                GameData.m_GameManager.JoinMatchRoomInPos(data);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 解析网络数据
    /// </summary>
    /// <param name="buffer"></param>
    public void OnUdpMessage(CReadPacket buffer)
    {
        if (buffer == null)
            return;
        uint protocol = buffer.GetMessageID();
        Dictionary<string, object> data = buffer.data;
        switch (protocol)
        {
            case NetProtocol.SYNC_KEY:
                GameData.m_GameManager.SyncKey(data);
                break;
            case NetProtocol.START:
                GameData.m_IsGame = true;
                GameData.m_GameManager.m_UIManager.m_UpdateEmbattleUICallback();
                object[] playerObj = data["info"] as object[];
                GameData.m_GameManager.CreateAllPlayer(playerObj);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void OnDisconnect()
    {
        if (m_TcpClient == null)
            return;
        m_TcpClient.OnMessage -= OnTcpMessage;
        m_TcpClient.Disconnect();
        m_TcpClient = null;
        if (m_UdpClient == null)
            return;
        m_UdpClient.OnMessage -= OnUdpMessage;
        m_UdpClient.Disconnect();
        m_UdpClient = null;
    }
}
