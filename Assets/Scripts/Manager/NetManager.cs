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
    //网络连接
    private AsyncUdpClient m_Client;

    /// <summary>
    /// 开始网络连接
    /// </summary>
    public void InitClient()
    {
        m_Client = new AsyncUdpClient("192.168.3.27", 8889);
        m_Client.OnMessage += OnMessage;
    }

    //每帧处理网络数据
    public void UpdateNet()
    {
        m_Client.UpdateNet();
        GameData.m_PingTime += GameData.m_FixFrameLen;
        if (GameData.m_PingTime >= Fix64.FromRaw(40000))
        {
            m_Client.AsyncSendPing();
            GameData.m_PingTime = Fix64.Zero;
        }
    }

    /// <summary>
    /// 发送网络数据
    /// </summary>
    /// <param name="buffer"></param>
    public void Send(CWritePacket buffer)
    {
        m_Client.AsyncSendData(buffer);
    }

    /// <summary>
    /// 解析网络数据
    /// </summary>
    /// <param name="buffer"></param>
    public void OnMessage(CReadPacket buffer)
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
        m_Client.OnMessage -= OnMessage;
        m_Client.Disconnect();
        m_Client = null;
    }
}
