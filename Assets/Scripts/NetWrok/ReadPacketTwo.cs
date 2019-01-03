using System;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 读取数据包
/// </summary>
public class CReadPacketTwo : CPacketTwo
{
    //数据列表
    public List<CReadPacket> m_PacketList;
    //读取数据
    private BinaryReader m_Reader;
    //被跳过次数
    private int m_Skip = 0;
    public CReadPacketTwo(byte[] bData, int len = 0) : base(0, 0, 0, 0, 0)
    {
        if (bData != null)
        {
            m_Stream = new MemoryStream(bData, 0, len);
            m_Reader = new BinaryReader(m_Stream);
            m_Stream.Seek(0, SeekOrigin.Begin);
            m_Cmd = m_Reader.ReadByte();
            m_Win = m_Reader.ReadByte();
            m_Sn = ReadInt();
            m_NextSn = ReadInt();
            m_Ts = ReadLong();
            ReadData();
        }
    }

    /// <summary>
    /// 解析实际协议数据
    /// </summary>
    private void ReadData()
    {
        while (m_Stream.Length - m_Stream.Position > sizeof(int))
        {
            int messageLength = ReadInt();
            m_Stream.Position = m_Stream.Position - sizeof(int);
            if (m_Stream.Length - m_Stream.Position >= messageLength)
            {
                byte[] message = m_Reader.ReadBytes(messageLength);
                CReadPacket readPacket = new CReadPacket(message, message.Length);
                readPacket.ReadData();
                m_PacketList.Add(readPacket);
            }
        }
    }


    /// <summary>
    /// 读取int类型数据
    /// </summary>
    /// <returns></returns>
    private int ReadInt()
    {
        int nValue = m_Reader.ReadInt32();
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        return nValue;
    }

    /// <summary>
    /// 读取long类型数据
    /// </summary>
    /// <returns></returns>
    private long ReadLong()
    {
        long nValue = m_Reader.ReadInt64();
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        return nValue;
    }

    /// <summary>
    /// 增加被跳过次数
    /// </summary>
    public void IncreaSkip()
    {
        m_Skip++;
    }
}

