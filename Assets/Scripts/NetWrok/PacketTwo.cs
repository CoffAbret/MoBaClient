using System;
using UnityEngine;
using System.IO;

public class CPacketTwo
{
    //��ǰ�����
    protected int m_Sn = 0;
    //˳��Ӱ���һ���������
    protected int m_NextSn = 0;
    //ʱ���
    protected long m_Ts = 0;
    //����ָ��
    protected byte m_Cmd = 0;
    //���ڴ�С
    protected byte m_Win = 0;
    //��д���ݵ��ڴ���
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

