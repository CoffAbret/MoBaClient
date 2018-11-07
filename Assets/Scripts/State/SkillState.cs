﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能攻击状态
/// </summary>
public class SkillState : BaseState
{
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //动画状态机
    private Animator m_Animator;
    //特效
    private GameObject m_AniEffect;
    //状态机参数名
    private string m_StateParameter = "State";
    //计算伤害时间
    private Fix64 m_CalcDamageTime = Fix64.FromRaw(3000);
    //播放特效时间
    private Fix64 m_PlayEffectTime = Fix64.FromRaw(300);
#endif
    #endregion
    //普攻段数
    private int m_AttackSegments = 3;
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(Player player, string parameter = null)
    {
        base.OnInit(player, parameter);
        if (m_Player == null)
            return;
        m_Player.m_SkillIndex = int.Parse(m_Parameter);
        m_Player.m_SkillNode = m_Player.m_CharData.GetSkillNode(m_Player.m_SkillIndex);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = player.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (m_Player == null)
            return;
        if (m_Player.m_SkillNode == null)
            return;
        Player target = m_Player.FindTarget(m_Player.m_SkillNode);
        if (target != null)
        {
            FixVector3 relativePos = target.m_Pos - m_Player.m_Pos;
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_Player.m_Rotation = new FixVector3((Fix64)rotation.eulerAngles.x, (Fix64)rotation.eulerAngles.y, (Fix64)rotation.eulerAngles.z);
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
                m_Player.m_VGo.transform.rotation = rotation;
            m_Player.m_Angles = new FixVector3((Fix64)m_Player.m_VGo.transform.forward.x, (Fix64)m_Player.m_VGo.transform.forward.y, (Fix64)m_Player.m_VGo.transform.forward.z).GetNormalized();
            #endregion
        }
        //为了匹配状态机参数这儿减去普攻段数
        //m_Player.m_SkillIndex = m_Player.m_SkillIndex - m_AttackSegments;
        if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill)
            m_Player.m_IsSkillMove = true;
        else if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill2)
            m_Player.m_IsSkillMove = true;
        else
            m_Player.m_IsSkillMove = false;
        m_Player.m_IsSkill = true;
        m_Player.m_IsCalcDamage = false;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, m_Player.m_SkillIndex);
            m_AniEffect = GameObject.Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}/{2}", GameData.m_EffectPath, m_Player.m_CharData.m_HeroName, m_Player.m_SkillNode.spell_motion)));
            if (m_Player.m_IsSkillMove)
            {
                m_AniEffect.transform.parent = m_Player.m_VGo.transform;
                m_AniEffect.transform.localPosition = Vector3.zero;
                m_AniEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                m_AniEffect.transform.localPosition = m_Player.m_VGo.transform.localPosition;
                m_AniEffect.transform.localRotation = m_Player.m_VGo.transform.localRotation;
            }
            m_AniEffect.transform.localScale = Vector3.one;
            m_AniEffect.SetActive(true);
            m_Player.m_IsPlayEffect = true;
            Delay delay = new Delay();
            delay.Init((Fix64)m_Player.m_SkillNode.efficiency_time, delegate { if (m_AniEffect != null) GameObject.DestroyImmediate(m_AniEffect); });
            GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
        }
        #endregion
    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (m_Player == null)
            return;
        if (m_Player.m_SkillNode == null)
            return;
        m_Player.m_IntervalTime += GameData.m_FixFrameLen;
        if (m_Player.m_IsSkillMove)
        {
            //技能位移直接设置方向即可
            m_Player.m_Pos = m_Player.m_Pos + m_Player.m_Angles * m_Player.m_SkillSpeed;
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
            {
                m_Player.m_VGo.transform.position = m_Player.m_Pos.ToVector3();
            }
            #endregion
        }
        if (!m_Player.m_IsSkill)
            return;
        if (m_Player.m_IntervalTime >= ((Fix64)(m_Player.m_SkillNode.animatorTime * m_CalcDamageTime)) && !m_Player.m_IsCalcDamage)
        {
            m_Player.CalcDamage(m_Player.m_SkillNode);
            m_Player.m_IsCalcDamage = true;
        }
        if (m_Player.m_IntervalTime >= (Fix64)m_Player.m_SkillNode.animatorTime)
            OnExit();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_Player == null)
            return;
        m_Player.m_IsSkill = false;
        m_Player.m_IsSkillMove = false;
        m_Player.m_SkillIndex = 0;
        m_Player.m_SkillNode = null;
        m_Player.m_IntervalTime = Fix64.Zero;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
        //if (GameData.m_GameManager != null && GameData.m_GameManager.m_LogMessage != null)
        //    GameData.m_GameManager.m_LogMessage.text += string.Format("{0}:{1},", GameData.m_GameFrame, m_Player.m_VGo.transform.position);
        //if (GameData.m_GameManager != null && GameData.m_GameManager.m_LogMessage != null)
        //    GameData.m_GameManager.m_LogMessage.text += string.Format("{0}:{1},", GameData.m_GameFrame, m_Player.m_Pos);
    }
}