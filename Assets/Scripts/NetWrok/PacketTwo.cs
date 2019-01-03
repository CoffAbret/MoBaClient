using System;
using UnityEngine;
using System.IO;

public class CPacketTwo
{
    //当前的序号
    protected int m_Sn = 0;
    //顺序接包下一个包的序号
    protected int m_NextSn = 0;
    //时间戳
    protected long m_Ts = 0;
    //操作指令
    protected byte m_Cmd = 0;
    //窗口大小
    protected byte m_Win = 0;
    //读写数据的内存流
    protected MemoryStream m_Stream;
    public CPacketTwo(int sn, int nextSn, long ts, byte cmd, byte win)
    {
        m_Sn = sn;
        m_NextSn = nextSn;
        m_Ts = ts;
        m_Cmd = cmd;
        m_Win = win;
    }
    public int GetSn()
    {
        return m_Sn;
    }

    public long GetTs()
    {
        return m_Ts;
    }

    public byte[] GetPacketByte()
    {
        return m_Stream.ToArray();
    }

    public int GetPos() { return (int)m_Stream.Position; }
}

