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
    //延时执行方法
    ActionCallback m_Callback;
    //是否开启
    public bool m_Enable;
    public void updateLogic()
    {
        m_FixElapseTime = m_FixElapseTime + GameData.m_FixFrameLen;
        if (m_FixElapseTime >= m_FixPlanTime)
        {
            if (m_Callback != null)
                m_Callback();
            m_Enable = false;
        }
    }

    /// <summary>
    /// 初始化延时
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void Init(Fix64 time, ActionCallback callback)
    {
        m_Enable = true;
        m_FixPlanTime = time;
        m_Callback = callback;
    }

    public void Destory()
    {
        m_FixPlanTime = Fix64.Zero;
        m_FixElapseTime = Fix64.Zero;
        m_Callback = null;
        m_Enable = false;
    }
}

public delegate void ActionCallback();
