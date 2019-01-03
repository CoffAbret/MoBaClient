using System;
using System.IO;
using UnityEngine;
using System.Net;
/// <summary>
/// 写入数据包
/// </summary>
public class CWritePacketTwo : CPacketTwo
{
    //写入数据
    private BinaryWriter m_Writer;
    //重传次数
    public int m_Rto = 0;
    //实体包的数据
    public byte[] m_Data = null;
    public CWritePacketTwo(int sn, int nextSn, long ts, byte cmd, byte win) : base(sn, nextSn, ts, cmd, win)
    {
        m_Stream = new MemoryStream();
        m_Writer = new BinaryWriter(m_Stream);
        m_Writer.Write(cmd);
        m_Writer.Write(win);
        WriteInt(sn);
        WriteInt(nextSn);
        WriteLong(ts);
    }

    /// <summary>
    /// 写入int类型
    /// </summary>
    /// <param name="nValue"></param>
    public void WriteInt(int nValue)
    {
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        m_Writer.Write(nValue);
    }

    /// <summary>
    /// 写入long类型
    /// </summary>
    /// <param name="nValue"></param>
    public void WriteLong(long nValue)
    {
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        m_Writer.Write(nValue);
    }
}

