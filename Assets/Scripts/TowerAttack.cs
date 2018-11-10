using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭塔子弹
/// </summary>
public class TowerAttack
{
    //位置
    public FixVector3 m_Pos = FixVector3.Zero;
    //攻击目标
    public Player m_TargetPlayer;
    //攻击距离
    public Fix64 m_AttackDistince = Fix64.FromRaw(1000);
    //攻击间隔
    public Fix64 m_AttackTime = Fix64.FromRaw(500);
    //攻击特效
    public GameObject m_Attack;
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(GameObject towerGo, Player targetPlayer)
    {
        m_TargetPlayer = targetPlayer;
        m_Attack = towerGo.transform.Find("attack0").gameObject;
        m_Attack.SetActive(true);
        m_Pos = new FixVector3((Fix64)towerGo.transform.position.x, (Fix64)towerGo.transform.position.y, (Fix64)towerGo.transform.position.z);
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_Attack == null || !m_Attack.activeSelf)
            return;
        FixVector3 fixAttackPos = new FixVector3((Fix64)m_Attack.transform.position.x, (Fix64)m_Attack.transform.position.y, (Fix64)m_Attack.transform.position.z);
        Fix64 distince = FixVector3.Distance(m_TargetPlayer.m_Pos, fixAttackPos);
        if (distince < m_AttackDistince)
        {
            m_TargetPlayer.FallDamage(50);
            Destroy();
        }
        else
        {
            //普通攻击自动改变朝向
            FixVector3 relativePos = m_TargetPlayer.m_Pos - new FixVector3((Fix64)m_Attack.transform.position.x, (Fix64)m_Attack.transform.position.y, (Fix64)m_Attack.transform.position.z);
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_Attack.transform.rotation = rotation;
            m_Attack.transform.position += m_Attack.transform.forward * (float)m_AttackTime;
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        m_Pos = FixVector3.Zero;
        m_TargetPlayer = null;
        if (m_Attack != null)
        {
            m_Attack.SetActive(false);
            m_Attack.transform.localPosition = new Vector3(0, 1, 0);
            m_Attack.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
