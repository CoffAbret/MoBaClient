using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 延时执行
/// </summary>
public class Delay
{
    //时长
    Fix64 m_FixPlanTime = Fix64.Zero;
    //累计时间
    Fix64 m_FixElapseTime = Fix64.Zero;
    //英雄复活延时
    public delegate void ResurgenceCallback(PlayerData data);
    //英雄复活延时
    ResurgenceCallback m_ResurgenceCallback = null;
    //复活英雄数据
    PlayerData m_ResurgencePlayerData = null;
    //销毁物体
    GameObject m_DestoryGo = null;
    //是否开启
    public bool m_Enable;
    public void updateLogic()
    {
        m_FixElapseTime = m_FixElapseTime + GameData.m_FixFrameLen;
        if (m_FixElapseTime >= m_FixPlanTime)
        {
            if (m_DestoryGo != null)
                GameObject.DestroyImmediate(m_DestoryGo);
            if (m_ResurgenceCallback != null && m_ResurgencePlayerData != null)
                m_ResurgenceCallback(m_ResurgencePlayerData);
            m_Enable = false;
        }
    }

    /// <summary>
    /// 初始化延时
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void InitResurgence(PlayerData playerData, Fix64 time, ResurgenceCallback callback)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_ResurgencePlayerData = playerData;
        m_ResurgenceCallback = callback;
    }

    /// <summary>
    /// 初始化延时
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void InitDestory(GameObject go, Fix64 time)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_DestoryGo = go;
    }

    public void Destory()
    {
        m_FixPlanTime = Fix64.Zero;
        m_FixElapseTime = Fix64.Zero;
        m_DestoryGo = null;
        m_ResurgenceCallback = null;
        m_ResurgencePlayerData = null;
        m_Enable = false;
    }
}