﻿using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    //接收消息数据
    private List<CReadPacket> m_ReceivePacketList;
    public static bool loggedIn = false;
    //网络数据派发
    public delegate void MessageHandle(CReadPacket msg);
    public event MessageHandle OnMessage;
    // Use this for initialization
    public SocketClient()
    {
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
        m_ReceivePacketList = new List<CReadPacket>();
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        this.Close();
        reader.Close();
        memStream.Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port)
    {
        client = null;
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Close(); Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        outStream = client.GetStream();
        client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            ushort msglen = (ushort)message.Length;
            //writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                //NetworkStream stream = client.GetStream();
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogError("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
            lock (client.GetStream())
            {         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            //PrintBytes();
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Close();   //关掉客户端链接
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        Debug.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        memStream.Seek(0, SeekOrigin.Begin);
        MemoryStream ms = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(ms);
        writer.Write(reader.ReadBytes(length));
        ms.Seek(0, SeekOrigin.Begin);
        OnReceivedMessage(ms);
        memStream.SetLength(0);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
        CReadPacket readPacket = new CReadPacket(message, message.Length);
        readPacket.ReadData();
        m_ReceivePacketList.Add(readPacket);
    }


    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
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
        WriteMessage(writePacket.GetPacketByte());
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
        loggedIn = false;
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect()
    {
        ConnectServer("192.168.3.42", 8889);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(CWritePacket data)
    {
        SessionSend(data.GetPacketByte());
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