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
        stream.Close();
        stream = null;
    }
    public UInt32 GetMessageID()
    {
        return (UInt32)GetPacketID();
    }

    public string GetLogMessageID()
    {
        return "|0x" + Convert.ToString(GetPacketID(), 16);
    }

    //restart Read
    public void ReStartRead() { stream.Position = 0; }
    //json 转化成相应类型的值
    public int GetInt(string key)
    {
        int valu = -1;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!int.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;

    }
    public short GetShort(string key)
    {
        short valu = -1;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!short.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。");
        }
        return valu;

    }
    public UInt32 GetUint32(string key)
    {
        UInt32 valu = 0;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!UInt32.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;

    }
    public float GetFloat(string key)
    {
        float valu = -1;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!float.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;
    }
    public string GetString(string key)
    {
        string valu = "";
        if (data == null) return valu;
        if (data.ContainsKey(key))
            valu = data[key].ToString();
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;
    }
    public double GetDouble(string key)
    {
        double valu = -1;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!double.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;

    }
    public long GetLong(string key)
    {
        long valu = -1;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!long.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;
    }
    public byte ReadByte(string key)
    {
        byte valu = 255;
        if (data == null) return valu;
        if (data.ContainsKey(key))
        {
            if (!byte.TryParse(data[key].ToString(), out valu))
            {
                GameDebug.Log("string 不合法 + " + data[key], DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.Log(key + " 键值不存在。。。。。", DebugLevel.Exception);
        }
        return valu;
    }
    //read short
    public short ReadShort()
    {
        if (GetPos() >= 0)
        {
            //int nSize = sizeof(short);
            //if ((m_nPos + nSize) > GetPacketLen())
            //{
            //    Debug.Log("GetShort error!");
            //    return 0;
            //}
            return ReadShortUnlimite();
        }
        return 0;
    }

    public int ReadIntEx()
    {
        if (GetPos() >= 0)
        {
            //int pos = GetPos();
            //Debug.LogError(pos);
            return ReadIntUnlimite();
        }
        return 0;
    }


    //read int
    public int ReadInt()
    {
        if (GetPos() >= 0 && GetPos() < GetPacketLen())
        {
            int nSize = sizeof(int);
            if ((GetPos() + nSize) > GetPacketLen())
            {
                Debug.Log("GetInt error!");
                return 0;
            }
            //int nValue = BitConverter.ToInt32(m_bDataList, m_nPos);
            //m_nPos += nSize;
            //if ( BitConverter.IsLittleEndian )
            byte[] tmpBuffer = new byte[nSize];
            stream.Read(tmpBuffer, 0, nSize);
            int nValue = BitConverter.ToInt32(tmpBuffer, GetPos());
            return System.Net.IPAddress.NetworkToHostOrder(nValue);
        }
        return 0;
    }

    //read string
    public String ReadString()
    {
        if (GetPos() >= 0)
        {
            int nBlobLen = (int)(m_nPacketLen - 8);

            // nBlobLen -= 2;
            //Debug.LogErrorFormat("ReadPacket >>> 0x{0}, {1}", Convert.ToString(GetPacketID(), 16), nBlobLen);
            byte[] bDataTemp = ReadBlob(nBlobLen);

            if (nBlobLen > 0)
            {
                string tempstr = "";
                // tempstr = System.Text.Encoding.ASCII.GetString( bDataTemp , 0 , nBlobLen );
                //  tempstr = Decompress(tempstr, DataDefine.datakey);

                //char[] tempstr = System.Text.Encoding.ASCII.GetChars( System.Text.Encoding.ASCII.GetBytes("6ICB6buR5ZOm77yM6ZmI6Zu344CC") );

                //byte[] arrtemp =  Convert.FromBase64CharArray( tempstr , 0,tempstr.Length);
                //string content = System.Text.Encoding.UTF8.GetString( arrtemp );
                tempstr = Decompress(bDataTemp, nBlobLen, DataDefine.datakey);
                return tempstr;
                //  return Decompress(tempstr, DataDefine.datakey);
            }
        }
        return "";
    }
    // 解密
    public string Decompress(byte[] pIn, int pInlength, string key)

    {
        if (DataDefine.isEFS == 1)
        {
            //  key = "bloodgod20160516";
            byte[] mBuffer = new byte[pInlength];
            byte[] pkey = System.Text.Encoding.ASCII.GetBytes(key);//System.Text.Encoding.UTF8.GetBytes(key);
            int len = pkey.Length;
            //  byte[] pIn = System.Text.Encoding.ASCII.GetBytes(strContent);  //System.Text.Encoding.UTF8.GetBytes(strContent);
            for (int i = 0; i < pInlength; i++)
            {
                mBuffer[i] = (byte)(((int)pIn[i]) ^ ((int)pkey[i % len]));
            }
            // Array.Copy(key, 0, pkey, 0, key.Length);

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

    //read blob
    public byte[] ReadBlob(int nBlobLength)
    {
        //if ( m_nPos >=0  )
        //{
        //first read bloglength
        //short nBlobLenT = ReadShort();
        //if ( (m_nPos+nBlobLenT) > GetPacketLen() )
        //{
        //	Debug.Log( "Read Blob Very Large!" );
        //	nBlobLength=0;
        //	return null;
        //}

        //send read byte[]
        if (nBlobLength > 0)
        {
            //nBlobLength = nBlobLenT;

            byte[] bList = new byte[nBlobLength];
            //Array.Copy(m_bDataList, m_nPos, bList, 0, nBlobLength);
            //m_nPos += nBlobLength;
            int ret = stream.Read(bList, 0, nBlobLength);
            if (ret >= 0)
            {

            }
            return bList;
        }
        //}
        //nBlobLength=0;
        return null;
    }


    //read short Unlimite
    private short ReadShortUnlimite()
    {
        int nSize = sizeof(short);
        ////Debug.Log("nPos:"+m_nPos);
        //short nValue = BitConverter.ToInt16(m_bDataList, m_nPos);
        //m_nPos += nSize;
        ////Debug.Log("Short:"+nValue);

        byte[] tmpBuffer = new byte[nSize];
        stream.Read(tmpBuffer, 0, nSize);
        short nValue = BitConverter.ToInt16(tmpBuffer, 0);
        return System.Net.IPAddress.NetworkToHostOrder(nValue);
    }

    private int ReadIntUnlimite()
    {
        int nSize = sizeof(int);
        ////Debug.Log("nPos:"+m_nPos);
        //int nValue = BitConverter.ToInt32(m_bDataList, m_nPos);
        //m_nPos += nSize;
        ////Debug.Log("Short:"+nValue);
        byte[] tmpBuffer = new byte[nSize];
        stream.Read(tmpBuffer, 0, nSize);
        int nValue = BitConverter.ToInt32(tmpBuffer, 0);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        //Debug.Log("ReadIntUnlimite:" + nValue);
        return nValue;
    }
}

