using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class IoBuffer
{
    MemoryStream stream = null;
    BinaryWriter writer = null;
    BinaryReader reader = null;

    public long Position
    {
        get { return stream.Position; }
        set { stream.Position = value; }
    }

    public IoBuffer()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public IoBuffer(byte[] data)
    {
        if (data != null)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream);
        }
        else
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }
    }

    public void Close()
    {
        if (writer != null) writer.Close();
        if (reader != null) reader.Close();

        stream.Close();
        writer = null;
        reader = null;
        stream = null;
    }

    public void WriteByte(byte v)
    {
        writer.Write(v);
    }

    public void WriteShort(short v)
    {
        v = System.Net.IPAddress.NetworkToHostOrder(v);
        byte[] bDataListT = BitConverter.GetBytes(v);
        writer.Write(bDataListT, 0, bDataListT.Length);
    }

    public void WriteInt(int v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(temp);
    }

    public void WriteLong(long v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(temp);
    }

    public void WriteFloat(float v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToSingle(temp, 0));
    }

    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        writer.Write(BitConverter.ToDouble(temp, 0));
    }

    public void WriteString(string v)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(v);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    public void WriteBytes(byte[] v)
    {
        writer.Write(v);
    }

    public byte ReadByte()
    {
        return reader.ReadByte();
    }

    public int ReadInt()
    {
        int nSize = sizeof(int);
        byte[] tmpBuffer = new byte[nSize];
        stream.Read(tmpBuffer, 0, nSize);
        int nValue = BitConverter.ToInt32(tmpBuffer, 0);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        return nValue;
    }

    public short ReadShort()
    {
        int nSize = sizeof(short);
        byte[] tmpBuffer = new byte[nSize];
        stream.Read(tmpBuffer, 0, nSize);
        short nValue = BitConverter.ToInt16(tmpBuffer, 0);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        return nValue;
    }

    public long ReadLong()
    {
        int nSize = sizeof(long);
        byte[] tmpBuffer = new byte[nSize];
        stream.Read(tmpBuffer, 0, nSize);
        long nValue = BitConverter.ToInt64(tmpBuffer, 0);
        nValue = System.Net.IPAddress.NetworkToHostOrder(nValue);
        return nValue;
    }

    public float ReadFloat()
    {
        byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
        Array.Reverse(temp);
        return BitConverter.ToSingle(temp, 0);
    }

    public double ReadDouble()
    {
        byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
        Array.Reverse(temp);
        return BitConverter.ToDouble(temp, 0);
    }

    public string ReadString()
    {
        int len = ReadInt();
        byte[] buffer = new byte[len];
        buffer = reader.ReadBytes(len);
        return Encoding.UTF8.GetString(buffer);
    }

    public byte[] ReadBytes(int length)
    {
        return reader.ReadBytes(length);
    }

    public byte[] ToBytes()
    {
        writer.Flush();
        return stream.ToArray();
    }

    public void Flush()
    {
        writer.Flush();
    }

    public long RemainingBytes()
    {
        return stream.Length - stream.Position;
    }

    public void SeekBegin()
    {
        stream.Seek(0, SeekOrigin.Begin);
    }

    public void SeekEnd()
    {
        stream.Seek(0, SeekOrigin.End);
    }
}