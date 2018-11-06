using System;
using UnityEngine;
using System.IO;

public class CPacket
{
    protected uint m_nPacketID;
    protected int m_nPacketLen;
    protected MemoryStream stream;
    public CPacket(uint nID, int nLen)
    {
        m_nPacketID = nID;
        m_nPacketLen = nLen;
    }
    public uint GetPacketID()
    {
        return m_nPacketID;
    }

    public string GetLogPacketID()
    {
        return "|0x" + Convert.ToString(GetPacketID(), 16);
    }

    public int GetPacketLen()
    {
        return m_nPacketLen;
    }

    public byte[] GetPacketByte()
    {
        return stream.ToArray();
    }

    public int GetPos() { return (int)stream.Position; }
    //big to little endian
    protected short BigToLittleEndian(short nValue)
    {
        return (short)(((((short)(nValue) & 0xff00) >> 8) |
                                   (((short)(nValue) & 0x00ff) << 8)));
    }

    //big to little endian
    protected int BigToLittleEndian(int nValue)
    {
        return (int)(((((int)(nValue) & 0xff000000) >> 24) |
                                    (((int)(nValue) & 0x00ff0000) >> 8) |
                                     (((int)(nValue) & 0x0000ff00) << 8) |
                                     (((int)(nValue) & 0x000000ff) << 24)));
    }
}

