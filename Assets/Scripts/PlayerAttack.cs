using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色子弹
/// </summary>
public class PlayerAttack
{
    //位置
    public FixVector3 m_Pos = FixVector3.Zero;
    //朝向
    public FixVector3 m_Angles = FixVector3.Zero;
    //攻击距离
    public Fix64 m_AttackDistince = Fix64.FromRaw(200);
    //攻击间隔
    public Fix64 m_AttackSpeed = Fix64.FromRaw(100);
    //攻击者
    public Player m_AttackPlayer;
    //攻击技能
    public SkillNode m_SkillNode;
    //子弹是否有效
    public bool m_IsActive = false;
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(Player player, SkillNode node)
    {
        m_AttackPlayer = player;
        m_SkillNode = node;
        m_Pos = player.m_Pos;
        m_Angles = player.m_Angles;
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_AttackPlayer == null)
            return;

    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destory()
    {

    }
}
