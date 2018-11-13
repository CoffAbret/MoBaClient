using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色AI
/// </summary>
public class PlayerAI
{
    private Player m_Player;
    //寻路最小距离
    private Fix64 m_distince = Fix64.FromRaw(2000);
    //寻路参数
    private string m_Parameter;
    /// <summary>
    /// 初始化AI数据
    /// </summary>
    /// <param name="player"></param>
    public void OnInit(Player player)
    {
        m_Player = player;
    }

    /// <summary>
    /// 进入AI
    /// </summary>
    public void OnEnter()
    {
        if (m_Player == null || m_Player.m_PlayerData == null || m_Player.m_PlayerData.m_Type == 1 || m_Player.m_PlayerData.m_NaviPos == null)
            return;
        FixVector3 relativePos = m_Player.m_PlayerData.m_NaviPos - m_Player.m_Pos;
        Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
        m_Player.m_Rotation = new FixVector3((Fix64)rotation.eulerAngles.x, (Fix64)rotation.eulerAngles.y, (Fix64)rotation.eulerAngles.z);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Player.m_VGo.transform.rotation = rotation;
        m_Player.m_Angles = new FixVector3((Fix64)m_Player.m_VGo.transform.forward.x, (Fix64)m_Player.m_VGo.transform.forward.y, (Fix64)m_Player.m_VGo.transform.forward.z).GetNormalized();
        #endregion

        if (m_Player.m_PlayerData.m_CampId == 1)
            m_Parameter = string.Format("{0}#{1}#{2}", m_Player.m_Angles.x, m_Player.m_Angles.y, m_Player.m_Angles.z);
        if (m_Player.m_PlayerData.m_CampId == 2)
            m_Parameter = string.Format("{0}#{1}#{2}", -m_Player.m_Angles.x, -m_Player.m_Angles.y, -m_Player.m_Angles.z);
    }
    /// <summary>
    /// 每帧更新AI逻辑
    /// </summary>
    public void UpdateLogic()
    {
        if (m_Player == null || m_Player.m_PlayerData == null || m_Player.m_PlayerData.m_Type == 1)
            return;
        if (m_Player.m_IsDie)
            return;
        if (m_Player.m_IsAttack)
            return;
        if (m_Player.m_IsSkill)
            return;
        if (m_Player.m_IsHit)
            return;
        m_Player.m_SkillNode = m_Player.m_PlayerData.GetSkillNode(1);
        Player targetPlayer = m_Player.FindTarget(m_Player.m_SkillNode);
        Tower targetTower = m_Player.FindTowerTarget(m_Player.m_SkillNode);
        if (targetPlayer != null || targetTower != null)
        {
            m_Player.m_State = new AttackState();
            m_Player.m_State.OnInit(m_Player);
            m_Player.m_State.OnEnter();
        }
        else
        {
            m_Player.m_State = new MoveState();
            m_Player.m_State.OnInit(m_Player, m_Parameter);
            m_Player.m_State.OnEnter();
            Fix64 distince = FixVector3.Distance(m_Player.m_PlayerData.m_NaviPos, m_Player.m_Pos);
            if (distince <= m_distince)
            {
                m_Player.m_State = new MoveEndState();
                m_Player.m_State.OnInit(m_Player);
                m_Player.m_State.OnEnter();
            }
        }
    }
}
