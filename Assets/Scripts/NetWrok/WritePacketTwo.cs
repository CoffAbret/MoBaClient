using System;
using System.IO;
using UnityEngine;
using System.Net;
/// <summary>
/// д�����ݰ�
/// </summary>
public class CWritePacketTwo : CPacketTwo
{
    //д������
    private BinaryWriter m_Writer;
    //�ش�����
    public int m_Rto = 0;
    //ʵ���������
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
    /// д��int����
    /// </summary>
    /// <param name="nValue"></param>
    public void WriteInt(int nValue)
    {
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        m_Writer.Write(nValue);
    }

    /// <summary>
    /// д��long����
    /// </summary>
    /// <param name="nValue"></param>
    public void WriteLong(long nValue)
    {
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        m_Writer.Write(nValue);
    }
}

