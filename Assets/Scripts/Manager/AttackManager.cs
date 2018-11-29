using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 延时
/// </summary>
public class AttackManager
{
    //子弹列表
    public List<PlayerAttack> m_AttackList = new List<PlayerAttack>();

    /// <summary>
    /// 每帧执行逻辑
    /// </summary>
    public void UpdateAttack()
    {
        for (int i = 0; i < m_AttackList.Count; i++)
        {
            if (m_AttackList[i].m_IsActive)
                m_AttackList[i].UpdateLogic();
            else
            {
                m_AttackList[i].Destory();
                m_AttackList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 清理所有子弹
    /// </summary>
    public void DestoryAttack()
    {
        if (m_AttackList == null || m_AttackList.Count < 1)
            return;
        for (int i = 0; i < m_AttackList.Count; i++)
        {
            m_AttackList[i].Destory();
        }
        m_AttackList.Clear();
        m_AttackList = null;
    }
}
