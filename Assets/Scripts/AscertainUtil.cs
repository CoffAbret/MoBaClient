using System;
using System.Collections.Generic;
using UnityEngine;

public static class AscertainUtil {

    #region 王冲添加 容错解析Dictionary<string, object>类型
    public static Boolean TryGetBool(this Dictionary<string, object> dict, string key)
    {
        Boolean value = false;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!Boolean.TryParse(obj.ToString(), out value))
            {
                GameDebug.LogError("Bool type parse error!               " + key, DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static int TryGetInt (this Dictionary<string, object> dict, string key, int defaultValue = default(int))
    {
        int value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!int.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
                GameDebug.LogError("Int type parse error!               "+ key, DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find"+key, DebugLevel.Exception);
        }
        return value;
    }

    public static uint TryGetUint(this Dictionary<string, object> dict, string key, uint defaultValue = default(uint))
    {
        uint value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!uint.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
                GameDebug.LogError("uint type parse error!", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static Byte TryGetByte(this Dictionary<string, object> dict, string key, Byte defaultValue = default(Byte))
    {
        Byte value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!Byte.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
                GameDebug.LogError("Byte type parse error!", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static short TryGetShort(this Dictionary<string, object> dict, string key, short defaultValue = default(short))
    {
        short value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!short.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
                GameDebug.LogError("Short type parse error!", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static float TryGetFloat(this Dictionary<string, object> dict, string key, float defaultValue = default(float))
    {
        float value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (obj!=null)
            {
                if (!float.TryParse(obj.ToString(), out value))
                {
                    value = defaultValue;
                    GameDebug.LogError("Float type parse error!", DebugLevel.Exception);
                }
            }
            else
            {
                GameDebug.LogError("the value is null", DebugLevel.Exception);
            }
        
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static double TryGetDouble(this Dictionary<string, object> dict, string key, double defaultValue = default(double))
    {
        double value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!double.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
                GameDebug.LogError("Float type parse error!", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }

    public static long TryGetLong(this Dictionary<string, object> dict, string key, long defaultValue = default(long))
    {
        long value = defaultValue;
        if (dict != null && dict.ContainsKey(key))
        {
            object obj = dict[key];
            if (!long.TryParse(obj.ToString(), out value))
            {
                value = defaultValue;
                GameDebug.LogError("Long type parse error!", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static string TryGetString(this Dictionary<string, object> dict, string key, string defaultValue = "")
    {
        string value = defaultValue;
        object obj = null;
        if (dict != null && dict.TryGetValue(key,out obj))
        {      
            if (obj!=null)
            {
                value = obj.ToString();
            }
            else
            {
                GameDebug.LogError("Value值为NULL", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static string[] TryGetStringIntArr(this Dictionary<string, object> dict, string key, string[] defaultValue = default(string[]))
    {
        string[] value = defaultValue;
        object obj = null;
        if (dict != null && dict.Count > 0 && dict.TryGetValue(key, out obj))
        {
            if (obj != null)
            {
                string[] Intarr = obj as string[];
                if (Intarr != null)
                {
                    value = new string[Intarr.Length];
                    for (int m = 0; m < Intarr.Length; m++)
                    {
                        value[m] = Intarr[m];
                    }
                }
                else
                {
                    GameDebug.LogError("Value值为NULL或表结构不正确1" + key, DebugLevel.Exception);
                }
            }
            else
            {
                GameDebug.LogError("Value值为NULL 1", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find 1" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static int[] TryGetIntArr(this Dictionary<string, object> dict, string key, int[] defaultValue =default(int[]))
    {
        int[] value = defaultValue;
        object obj = null;
        if (dict != null && dict.Count > 0 && dict.TryGetValue(key, out obj))
        {
            if (obj!=null)
            {
                int[] Intarr = obj as int[];
                if (Intarr!=null)
                {
                    value = new int[Intarr.Length];
                    for (int m = 0; m < Intarr.Length; m++)
                    {
                        value[m] = Intarr[m];
                    }
                }
                else
                {
                    GameDebug.LogError("Value值为NULL或表结构不正确1"+ key, DebugLevel.Exception);
                }
            }
            else
            {
                GameDebug.LogError("Value值为NULL 1", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find 1" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static long [] TryGetLongArr(this Dictionary<string, object> dict, string key, long[] defaultValue = default(long[]))
    {
        long[] value = defaultValue;
        object obj = null;
        if (dict != null && dict.TryGetValue(key, out obj))
        {
            if (obj != null)
            {
                int[] Intarr = obj as int[];
                if (Intarr != null)
                {
                    value = new long[Intarr.Length];
                    for (int m = 0; m < Intarr.Length; m++)
                    {
                        value[m] = Intarr[m];
                    }
                }
                else
                {
                    GameDebug.LogError("Value值为NULL或表结构不正确2" + key, DebugLevel.Exception);
                }
            }
            else
            {
                GameDebug.LogError("Value值为NULL 2", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find 2" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static float[] TryGetFloatArr(this Dictionary<string, object> dict, string key, float[] defaultValue = default(float[]))
    {
        float[] value = defaultValue;
        object obj = null;
        if (dict != null && dict.TryGetValue(key, out obj))
        {
            if (obj != null)
            {
                float[] Intarr = obj as float[];
                if (Intarr != null)
                {
                    value = new float[Intarr.Length];
                    for (int m = 0; m < Intarr.Length; m++)
                    {
                        value[m] = Intarr[m];
                    }
                }
                else
                {
                    GameDebug.LogError("Value值为NULL或表结构不正确3" + key, DebugLevel.Exception);
                }
            }
            else
            {
                GameDebug.LogError("Value值为NULL 3", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find 3" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static double[] TryGetDoubleArr(this Dictionary<string, object> dict, string key, double[] defaultValue = default(double[]))
    {
        double[] value = defaultValue;
        object obj = null;
        if (dict != null && dict.Count > 0 && dict.TryGetValue(key, out obj))
        {
            if (obj != null)
            {
                double[] Intarr = obj as double[];
                if (Intarr != null)
                {
                    value = new double[Intarr.Length];
                    for (int m = 0; m < Intarr.Length; m++)
                    {
                        value[m] = Intarr[m];
                    }
                }
                else
                {
                    GameDebug.LogError("Value值为NULL或表结构不正确4" + key, DebugLevel.Exception);
                }
            }
            else
            {
                GameDebug.LogError("Value值为NULL 4", DebugLevel.Exception);
            }
        }
        else
        {
            GameDebug.LogError("don't find 4" + key, DebugLevel.Exception);
        }
        return value;
    }
    public static Vector3 TryGetToVector3(this Dictionary<string, object> dict, string key, Vector3 defaultValue=new Vector3())
    {
        double[] objs=null;
        if (dict.ContainsKey(key) && dict[key] != null)
        {
            objs = dict[key] as double[];
        }
        else
        {
            GameDebug.LogError("字段有问题" + key, DebugLevel.Exception);
        }
        if (objs!=null&& objs.Length==3)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                defaultValue = new Vector3(objs[0].ToString().StringToFloat(), objs[1].ToString().StringToFloat(), objs[2].ToString().StringToFloat());
            }
        }
        else
        {
            GameDebug.LogError("objs长度不足" + key, DebugLevel.Exception);
        }
        return defaultValue;
    }
    #endregion
}
