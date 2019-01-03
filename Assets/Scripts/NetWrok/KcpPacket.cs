using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class KcpPacket
{
    //当前的序号
    public int m_Sn = 0;
    //顺序接包下一个包的序号
    public int m_NextSn = 0;
    //时间戳
    public long m_Ts = 0;
    //操作指令
    public byte m_Cmd = 0;
    //窗口大小
    public byte m_Win = 0;
    //重传次数
    public int m_Rto = 0;
    //被跳过次数
    public int m_Skip = 0;
    //实体包的数据
    public byte[] m_Data = null;

    public void IncreaRto()
    {
        m_Rto++;
    }

    public void IncreaSkip()
    {
        m_Skip++;
    }

    public List<CReadPacket> Packet
    {
        get
        {
            if (m_Data == null || m_Data.Length == 0)
                return null;
            List<CReadPacket> ret = new List<CReadPacket>();
            IoBuffer io = new IoBuffer(m_Data);
            while (io.RemainingBytes() > sizeof(int))
            {
                int messageLength = io.ReadInt();
                io.Position = io.Position - sizeof(int);
                if (io.RemainingBytes() >= messageLength)
                {
                    byte[] message = io.ReadBytes(messageLength);
                    CReadPacket readPacket = new CReadPacket(message, message.Length);
                    readPacket.ReadData();
                    ret.Add(readPacket);
                }
            }
            return ret;
        }
    }
}

