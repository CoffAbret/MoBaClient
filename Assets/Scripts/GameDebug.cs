/*
文件名（File Name）:   GameDebug.cs

作者（Author）:    #高#

创建时间（CreateTime）:  2017-3-17 19:52:18
*/
using UnityEngine;
using System;
using System.IO;

public class GameDebug
{
    private static string logError = "";
    private static string logWarning = "";

    public static void Log(object message, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.Log(message);
        }
    }

    public static void LogFormat(string format, DebugLevel debugLevel = DebugLevel.None, params object[] arg)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.Log(string.Format(format, arg));
        }
    }

    public static void Log(object message, UnityEngine.Object context, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.Log(message, context);
        }
    }
    /// <summary>
    /// 警告
    /// </summary>
    /// <param name="message"></param>
    public static void LogWarning(object message, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.LogWarning(message);
            logWarning += message.ToString() + "\r\n";
        }
    }

    public static void LogWarning(object message, UnityEngine.Object context, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.LogWarning(message, context);
            logWarning += message.ToString() + "\r\n";
        }
    }
    /// <summary>
    /// 异常
    /// </summary>
    /// <param name="exception"></param>
    public static void LogException(Exception exception, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.LogException(exception);
        }
    }

    public static void LogException(Exception exception, UnityEngine.Object context, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.LogException(exception, context);
        }
    }

    public static void LogError(object message, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.LogError(message);
            logError += message.ToString() + "\r\n";
        }
    }

    public static void LogError(object message, UnityEngine.Object context, DebugLevel debugLevel = DebugLevel.None)
    {
        if (DataDefine.isPrintDebugInfo == debugLevel && DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            Debug.LogError(message, context);
        }
    }

    //////////////////////////////////////////保存日志////////////////////////////////////////
    public static void SaveLogError()
    {
        if (DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            WriteStringToFile("C:/Users/Administrator/Desktop/logError.txt", logError);
        }
    }

    public static void SaveLogWarning()
    {
#if UNITY_EDITOR
        if (DataDefine.isPrintDebugInfo != DebugLevel.None)
        {
            WriteStringToFile("C:/Users/Administrator/Desktop/logWarning.txt", logWarning);
        }
#endif
    }
    public static void WriteStringToFile(string fileName, string strData)
    {
#if UNITY_EDITOR
        try
        {
            using (StreamWriter writer = File.CreateText(fileName))
            {
                writer.Write(strData);
                writer.Close();
            }
        }
        catch (Exception e)
        {
            //   GameDebug.LogError("FileTools WriteStringToFile: " + e);
        }
#endif
    }
}
