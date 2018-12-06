using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 延时
/// </summary>
public class DelayManager
{
    //延时列表
    public List<Delay> m_DelayList = new List<Delay>();

    /// <summary>
    /// 每帧执行逻辑
    /// </summary>
    public void UpdateDelay()
    {
        for (int i = 0; i < m_DelayList.Count; i++)
        {
            if (m_DelayList[i].m_Enable)
                m_DelayList[i].UpdateLogic();
            else
            {
                m_DelayList[i].Destory();
                m_DelayList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 清理所有延时
    /// </summary>
    public void DestoryDelay()
    {
        if (m_DelayList == null || m_DelayList.Count < 1)
            return;
        for (int i = 0; i < m_DelayList.Count; i++)
        {
            m_DelayList[i].Destory();
        }
    }
}
