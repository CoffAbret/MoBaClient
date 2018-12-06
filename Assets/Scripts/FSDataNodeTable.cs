using System.IO;
using System.Collections.Generic;
using System.Text;
using Pathfinding.Serialization.JsonFx;
using System;
using UnityEngine;

public abstract class FSDataNodeBase
{
    public abstract void ParseJson(object jd);
}

public class FSDataNodeTable<T> : IDisposable where T : FSDataNodeBase, new()
{
    private string _data;
    private static FSDataNodeTable<T> mSingleton;
    private Dictionary<long, T> mDataNodeList;
    public Dictionary<long, T> DataNodeList { get { return mDataNodeList; } }

    /// <summary>
    /// 根据typeid得到节点
    /// </summary>
    /// <param name="typeId"></param>
    /// <returns></returns>
    public T FindDataByType(long typeId)
    {
        T retValue = default(T);
        if (mDataNodeList != null && mDataNodeList.TryGetValue(typeId, out retValue))
            return retValue;
        return null;
    }

    private string jsonNode;
    public void LoadJson(string jsonString)
    {
        Dictionary<string, object> json = (Dictionary<string, object>)Jsontext.ReadeData(jsonString);

        foreach (string jsonnode in json.Keys)
        {
            long typeId = 0;
            typeId = long.Parse(jsonnode);
            jsonNode = jsonnode;

            if (mDataNodeList == null)
            {
                mDataNodeList = new Dictionary<long, T>();
            }
            if (!mDataNodeList.ContainsKey(typeId))
            {
                T dataNode = new T();
                dataNode.ParseJson(json[jsonnode]);
                mDataNodeList.Add(typeId, dataNode);
            }
        }
    }

    public static FSDataNodeTable<T> GetSingleton()
    {
        if (mSingleton == null)
            mSingleton = new FSDataNodeTable<T>();

        return mSingleton;
    }

    public float[] ParseToFloatArray(object config)
    {
        if (config is Array)
        {
            Array arr = (Array)config;
            float[] floatArr = new float[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                floatArr[i] = float.Parse(arr.GetValue(i).ToString());
            }
            return floatArr;
        }
        return null;
    }

    public long[] ParseToLongArray(object config)
    {
        if (config is Array)
        {
            Array arr = (Array)config;
            long[] ret = new long[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                ret[i] = long.Parse(arr.GetValue(i).ToString());
            }
            return ret;
        }
        return null;
    }

    //托管和非托管内存管理，解决内存泄露问题
    private bool _disposed = false;
    ~FSDataNodeTable()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                OnDisposing();
            }
            _disposed = true;
        }
    }

    protected virtual void OnDisposing()
    {

    }
}

