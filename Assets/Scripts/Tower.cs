using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 显示层对象
/// </summary>
public class Tower
{
    //位置
    public FixVector3 m_Pos;
    //攻击目标
    public Player m_TargetPlayer;
    //攻击距离
    public Fix64 m_AttackDistince = Fix64.FromRaw(10000);
    //销毁延迟时间
    private Fix64 m_DestoryDelayTime = Fix64.FromRaw(5000);
    //显示对象
    public GameObject m_VGo;
    //塔攻击范围
    public GameObject m_Quan;
    //攻击特效
    public GameObject m_Attack;
    //技能间隔时间
    public Fix64 m_IntervalTime = Fix64.Zero;
    //所属阵营
    public int m_CampId;
    //子弹
    public TowerAttack m_TowerAttack;
    public Tower() { }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(int campId)
    {
        m_CampId = campId;
        if (m_CampId == 1)
        {
            m_VGo = GameObject.Find("Tower_Blue");
        }
        if (m_CampId == 2)
            m_VGo = GameObject.Find("Tower_Red");
        m_Quan = m_VGo.transform.Find("quan_hero").gameObject;
        m_Attack = m_VGo.transform.Find("attack0").gameObject;
        m_Pos = new FixVector3((Fix64)m_VGo.transform.position.x, (Fix64)m_VGo.transform.position.y, (Fix64)m_VGo.transform.position.z);
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {

        m_TargetPlayer = FindTarget(m_AttackDistince);
        if (m_TargetPlayer == null)
            m_Quan.SetActive(false);
        else
            m_Quan.SetActive(true);
        if (m_IntervalTime / (Fix64.One * 2) > Fix64.One)
            m_IntervalTime = Fix64.Zero;
        if (m_TargetPlayer != null && m_IntervalTime == Fix64.Zero)
        {
            TowerAttack attck = new TowerAttack();
            attck.Create(m_VGo, m_TargetPlayer);
            m_TowerAttack = attck;
        }
        if (m_TowerAttack != null)
            m_TowerAttack.UpdateLogic();

        m_IntervalTime += GameData.m_FixFrameLen;
    }

    /// <summary>
    /// 查找目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public Player FindTarget(Fix64 attackDistince)
    {
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i].m_CharData.m_CampId == m_CampId)
                continue;
            Fix64 distance = FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos);
            if (distance <= attackDistince)
            {
                return GameData.m_PlayerList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 判断击中目标
    /// </summary>
    public void CalcDamage(SkillNode skillNode)
    {

    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="playerAttack">攻击者</param>
    /// <param name="skillNode">攻击技能</param>
    public void FallDamage(Player playerAttack, SkillNode skillNode)
    {

    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        if (m_TowerAttack != null)
            m_TowerAttack.Destroy();
        m_TowerAttack = null;
        m_Pos = FixVector3.Zero;
        m_TargetPlayer = null;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Quan != null)
                GameObject.Destroy(m_Quan, (float)m_DestoryDelayTime);
            if (m_VGo != null)
                GameObject.Destroy(m_VGo, (float)m_DestoryDelayTime);
            m_Quan = null;
            m_VGo = null;
        }
        #endregion
    }
}
