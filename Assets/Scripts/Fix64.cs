using System;
using System.IO;
using UnityEngine;
/// <summary>
/// 定点数类
/// </summary>
public struct Fix64 : System.IEquatable<Fix64>
{
    private readonly long m_rawValue;
    public static readonly long FloatPrecision = 1000;
    public static readonly float PrecisionFactor = 0.001f;
    public static readonly Fix64 Zero = new Fix64();
    public static readonly Fix64 One = new Fix64(FloatPrecision);
    public Fix64(long value)
    {
        m_rawValue = value;
    }

    public static explicit operator float(Fix64 value)
    {
        return value.m_rawValue * PrecisionFactor;
    }

    public static explicit operator Fix64(float value)
    {
        return new Fix64((long)(Math.Round(value, 3) * FloatPrecision));
    }

    public static Fix64 operator +(Fix64 x, Fix64 y)
    {
        return new Fix64(x.m_rawValue + y.m_rawValue);
    }

    public static Fix64 operator -(Fix64 x, Fix64 y)
    {
        return new Fix64(x.m_rawValue - y.m_rawValue);
    }

    public static Fix64 operator *(Fix64 x, Fix64 y)
    {
        return new Fix64((x.m_rawValue * y.m_rawValue / FloatPrecision));
    }

    public static Fix64 operator /(Fix64 x, Fix64 y)
    {
        return new Fix64(x.m_rawValue / y.m_rawValue * FloatPrecision);
    }

    public static bool operator ==(Fix64 x, Fix64 y)
    {
        return x.m_rawValue == y.m_rawValue;
    }

    public static bool operator !=(Fix64 x, Fix64 y)
    {
        return x.m_rawValue != y.m_rawValue;
    }

    public static bool operator >(Fix64 x, Fix64 y)
    {
        return x.m_rawValue > y.m_rawValue;
    }

    public static bool operator <(Fix64 x, Fix64 y)
    {
        return x.m_rawValue < y.m_rawValue;
    }

    public static bool operator >=(Fix64 x, Fix64 y)
    {
        return x.m_rawValue >= y.m_rawValue;
    }

    public static bool operator <=(Fix64 x, Fix64 y)
    {
        return x.m_rawValue <= y.m_rawValue;
    }

    public static Fix64 operator -(Fix64 x)
    {
        return new Fix64(-x.m_rawValue);
    }

    public long RawValue { get { return m_rawValue; } }

    public static Fix64 FromRaw(long rawValue)
    {
        return new Fix64(rawValue);
    }

    public bool Equals(Fix64 other)
    {
        return other.m_rawValue == m_rawValue;
    }

    public override bool Equals(object obj)
    {
        return obj is Fix64 && ((Fix64)obj).m_rawValue == m_rawValue;
    }

    public override int GetHashCode()
    {
        return m_rawValue.GetHashCode();
    }

    public override string ToString()
    {
        return (m_rawValue * PrecisionFactor).ToString();
    }
}

public struct FixVector3
{
    public Fix64 x;
    public Fix64 y;
    public Fix64 z;

    public FixVector3(Fix64 x, Fix64 y, Fix64 z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public FixVector3(FixVector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public Fix64 this[int index]
    {
        get
        {
            if (index == 0)
                return x;
            else if (index == 1)
                return y;
            else
                return z;
        }
        set
        {
            if (index == 0)
                x = value;
            else if (index == 1)
                y = value;
            else
                y = value;
        }
    }

    public static FixVector3 Zero
    {
        get { return new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero); }
    }

    public Fix64 Magnitude
    {
        get { return (Fix64)Math.Round(Math.Sqrt((float)(x * x + y * y + z * z)), 3); }
    }

    public static explicit operator FixVector3(Vector3 ob)
    {
        return new FixVector3((Fix64)Math.Round(ob.x, 3), (Fix64)Math.Round(ob.y, 3), (Fix64)Math.Round(ob.z, 3));
    }

    public static FixVector3 operator +(FixVector3 a, FixVector3 b)
    {
        Fix64 x = a.x + b.x;
        Fix64 y = a.y + b.y;
        Fix64 z = a.z + b.z;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator -(FixVector3 a, FixVector3 b)
    {
        Fix64 x = a.x - b.x;
        Fix64 y = a.y - b.y;
        Fix64 z = a.z - b.z;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator *(Fix64 d, FixVector3 a)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        Fix64 z = a.z * d;
        return new FixVector3(x, y, z);
    }

    public static FixVector3 operator *(FixVector3 a, Fix64 d)
    {
        Fix64 x = a.x * d;
        Fix64 y = a.y * d;
        Fix64 z = a.z * d;
        return new FixVector3(x, y, z);
    }

    public static bool operator ==(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    public static bool operator !=(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
    }

    public static Fix64 SqrMagnitude(FixVector3 a)
    {
        return a.x * a.x + a.y * a.y + a.z * a.z;
    }

    public static Fix64 Distance(FixVector3 a, FixVector3 b)
    {
        return (Fix64)Vector3.Distance(a.ToVector3(), b.ToVector3());
    }

    public static Fix64 Angle(FixVector3 a, FixVector3 b)
    {
        return (Fix64)Vector3.Angle(a.ToVector3(), b.ToVector3());
    }

    public static Fix64 AngleTwo(FixVector3 a, FixVector3 b)
    {
        Fix64 cos = Dot(a, b) / (a.Magnitude * b.Magnitude);
        cos = cos < -Fix64.One ? -Fix64.One : (cos > Fix64.One ? Fix64.One : cos);
        return (Fix64)Math.Round(Math.Acos((float)cos), 3);
    }

    public static Fix64 Dot(FixVector3 a, FixVector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }


    public void Normalize()
    {
        Vector3 v3 = new Vector3((float)x, (float)y, (float)z);
        x = (Fix64)v3.normalized.x;
        y = (Fix64)v3.normalized.x;
        z = (Fix64)v3.normalized.x;
    }

    public FixVector3 GetNormalized()
    {
        FixVector3 v = new FixVector3(this);
        v.Normalize();
        return v;
    }

    public override string ToString()
    {
        return string.Format("x:{0} y:{1} z:{2}", (float)x, (float)y, (float)z);
    }

    public override bool Equals(object obj)
    {
        return obj is FixVector3 && ((FixVector3)obj) == this;
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode();
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float)x, (float)y, (float)z);
    }
}