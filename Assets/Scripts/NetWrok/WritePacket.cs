using System;
using System.IO;
using UnityEngine;
using System.Net;
public class CWritePacket : CPacket
{
    public CWritePacket(UInt32 mID) : base(mID, 0)
    {
        stream = new MemoryStream();
    }
    //Wite short
    public bool WriteShort(short nValue)
    {
        short nSize = sizeof(short);
        if ((GetPacketLen() + nSize) > DataDefine.MAX_PACKET_SIZE)
        {
            Debug.Log("WriteInt error!too Large!");
            return false;
        }
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        byte[] bDataListT = BitConverter.GetBytes(nValue);
        stream.Write(bDataListT, 0, bDataListT.Length);
        ChangeLength(nSize);

        return true;
    }

    //Wite int
    public bool WriteInt(int nValue)
    {
        int nSize = sizeof(int);
        if ((GetPacketLen() + nSize) > DataDefine.MAX_PACKET_SIZE)
        {
            Debug.Log("WriteInt error!too Large!");
            return false;
        }
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        byte[] bDataListT = BitConverter.GetBytes(nValue);
        stream.Write(bDataListT, 0, bDataListT.Length);
        ChangeLength(nSize);



        //byte[] temp = BitConverter.GetBytes(nValue);
        //Array.Reverse(temp);
        //stream.Write(temp, 0, temp.Length);
        //ChangeLength(nSize);
        return true;
    }

    //Wite int64


    //Write string
    public bool WriteString(String strValue)
    {
        if (strValue.Length <= 0) return false;
        byte[] bWriteString = System.Text.Encoding.UTF8.GetBytes(strValue);
        return WriteBlob(bWriteString, bWriteString.Length);
    }

    //Write blob
    public bool WriteBlob(byte[] bBlobList, int nBlobLength)
    {
        if (nBlobLength <= 0 || bBlobList == null) return false;
        int length = nBlobLength + 8;

        WriteInt(length);
        WriteInt((int)m_nPacketID);
        int nAllSize = nBlobLength * sizeof(byte);
        if ((GetPacketLen() + nAllSize) > DataDefine.MAX_PACKET_SIZE)
        {
            Debug.Log("WriteBlob error!too Large!");
            return false;
        }
        stream.Write(bBlobList, 0, nAllSize);
        ChangeLength(nAllSize);

        return true;
    }

    public string Compress(string strContent, string key)
    {
        //  key = "bloodgod20160516";
        byte[] mBuffer = new byte[strContent.Length];
        byte[] pkey = System.Text.Encoding.ASCII.GetBytes(key);//System.Text.Encoding.UTF8.GetBytes(key);
        int len = pkey.Length;
        byte[] pIn = System.Text.Encoding.ASCII.GetBytes(strContent);  //System.Text.Encoding.UTF8.GetBytes(strContent);
        for (int i = 0; i < strContent.Length; i++)
        {
            mBuffer[i] = (byte)(((int)pIn[i]) ^ ((int)pkey[i % len]));
        }
        // Array.Copy(key, 0, pkey, 0, key.Length);
        return System.Text.Encoding.ASCII.GetString(mBuffer);
    }
    private void ChangeLength(int nChange)
    {
        m_nPacketLen += nChange;
    }
}

