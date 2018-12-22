using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小兵AI
/// </summary>
public class MonsterAI
{
    private Monster m_Monster;
    //寻路参数
    private string m_Parameter;
    //寻路坐标
    private List<FixVector3> m_FixVectorPath;
    //寻路停止距离
    private Fix64 m_distince = Fix64.FromRaw(50);
    /// <summary>
    /// 初始化AI数据
    /// </summary>
    /// <param name="player"></param>
    public void OnInit(Monster monster)
    {
        m_Monster = monster;
        Path p = ABPath.Construct(m_Monster.m_Pos.ToVector3(), m_Monster.m_MonsterData.m_NaviPos.ToVector3(), OnPathComplete);
        AstarPath.StartPath(p);
    }

    public void OnPathComplete(Path p)
    {
        m_FixVectorPath = new List<FixVector3>();
        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            FixVector3 fixV3 = (FixVector3)(p.vectorPath[i]);
            m_FixVectorPath.Add(fixV3);
        }
    }

    /// <summary>
    /// 进入AI
    /// </summary>
    public void OnEnter()
    {
        if (m_Monster == null || m_Monster.m_MonsterData == null || m_Monster.m_MonsterData.m_NaviPos == null)
            return;
        FixVector3 relativePos = m_Monster.m_MonsterData.m_CampId == CampType.BLUE ? (m_Monster.m_MonsterData.m_NaviPos - m_Monster.m_Pos) : (m_Monster.m_Pos - m_Monster.m_MonsterData.m_NaviPos);
        Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
        m_Monster.m_Angles = relativePos.GetNormalized();
        m_Monster.m_Rotation = (FixVector3)(rotation.eulerAngles);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Monster.m_VGo.transform.rotation = rotation;
        #endregion

        if (m_Monster.m_MonsterData.m_CampId == CampType.BLUE)
            m_Parameter = string.Format("{0}#{1}#{2}", m_Monster.m_Angles.x, m_Monster.m_Angles.y, m_Monster.m_Angles.z);
        if (m_Monster.m_MonsterData.m_CampId == CampType.RED)
            m_Parameter = string.Format("{0}#{1}#{2}", -m_Monster.m_Angles.x, -m_Monster.m_Angles.y, -m_Monster.m_Angles.z);
    }

    /// <summary>
    /// 每帧更新AI逻辑
    /// </summary>
    public void UpdateLogic()
    {
        if (m_Monster == null || m_Monster.m_MonsterData == null)
            return;
        if (m_Monster.m_IsDie)
            return;
        if (m_Monster.m_IsAttack)
            return;
        if (m_Monster.m_IsSkill)
            return;
        if (m_Monster.m_IsHit)
            return;
        m_Monster.m_SkillNode = m_Monster.m_MonsterData.GetSkillNode(1);
        Fix64 attackDistance = (Fix64)m_Monster.m_SkillNode.aoe_long;
        BaseObject target = m_Monster.FindTarget(attackDistance);
        if (target != null)
        {
            m_Monster.m_State = new AttackState();
            m_Monster.m_State.OnInit(m_Monster, "1");
            m_Monster.m_State.OnEnter();
        }
        else
        {
            m_Monster.m_State = new MoveState();
            m_Monster.m_State.OnInit(m_Monster, m_Parameter);
            m_Monster.m_State.OnEnter();
            Fix64 distince = FixVector3.Distance(m_Monster.m_MonsterData.m_NaviPos, m_Monster.m_Pos);
            if (distince <= m_distince)
            {
                m_Monster.m_State = new MoveEndState();
                m_Monster.m_State.OnInit(m_Monster);
                m_Monster.m_State.OnEnter();
            }
        }
    }
}
