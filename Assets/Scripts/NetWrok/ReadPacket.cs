using System;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CReadPacket : CPacket
{
    public Dictionary<string, object> data;
    public CReadPacket(byte[] bData, int len = 0) : base(0, 0)
    {
        if (bData != null)
        {
            stream = new MemoryStream(bData, 0, len);
            stream.Seek(0, SeekOrigin.Begin);
            m_nPacketLen = ReadIntEx();
            m_nPacketID = (uint)ReadIntEx();
            ReadData();
        }
    }

    public void ReadData()
    {
        string tempstr = ReadString();
        object obj = Jsontext.ReadeData(tempstr);
        if (obj == null)
        {
            return;
        }
        data = obj as Dictionary<string, object>;
        if (data != null)
        {
            if (data.ContainsKey("msgid"))
            {
                m_nPacketID = data.TryGetUint("msgid");
            }
        }
    }
    public UInt32 GetMessageID()
    {
        return (UInt32)GetPacketID();
    }

    public string GetLogMessageID()
    {
        return "|0x" + Convert.ToString(GetPacketID(), 16);
    }

    public int ReadIntEx()
    {
        if (GetPos() >= 0)
        {
            return ReadIntUnlimite();
        }
        return 0;
    }

    public String ReadString()
    {
        if (GetPos() >= 0)
        {
            int nBlobLen = (int)(m_nPacketLen - 8);
            byte[] bDataTemp = ReadBlob(nBlobLen);

            if (nBlobLen > 0)
            {
                string tempstr = "";
                tempstr = Decompress(bDataTemp, nBlobLen, DataDefine.datakey);
                return tempstr;
            }
        }
        return "";
    }

    public string Decompress(byte[] pIn, int pInlength, string key)

    {
        if (DataDefine.isEFS == 1)
        {
            byte[] mBuffer = new byte[pInlength];
            byte[] pkey = System.Text.Encoding.ASCII.GetBytes(key);
            int len = pkey.Length;
            for (int i = 0; i < pInlength; i++)
            {
                mBuffer[i] = (byte)(((int)pIn[i]) ^ ((int)pkey[i % len]));
            }

            try
            {
                return System.Text.Encoding.UTF8.GetString(mBuffer);
            }
            catch (Exception)
            {
                Debug.LogError("服务器数据异常，此数据无法将字节转成字符串");
                return "";
            }

        }
        else
        {
            string temp = "";
            if (pIn != null)
            {
                try
                {
                    temp = System.Text.Encoding.UTF8.GetString(pIn);
                }
                catch (Exception)
                {
                    temp = "";
                }

            }
            return temp;
        }
    }

    public byte[] ReadBlob(int nBlobLength)
    {
        if (nBlobLength > 0)
        {
            byte[] bList = new byte[nBlobLength];
            int ret = stream.Read(bList, 0, nBlobLength);
            if (ret >= 0)
            {

            }
            return bList;
        }
        return null;
    }


    private int ReadIntUnlimite()
    {
        int nSize = sizeof(int);
        byte[] tmpBuffer = new byte[nSize];
        stream.Read(tmpBuffer, 0, nSize);
        int nValue = BitConverter.ToInt32(tmpBuffer, 0);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        return nValue;
    }
}

