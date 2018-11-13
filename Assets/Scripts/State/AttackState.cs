using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通攻击状态
/// </summary>
public class AttackState : BaseState
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
    private Fix64 m_CalcDamageTime = Fix64.FromRaw(2000);
    //普攻切换时间
    private Fix64 m_AttackTime = Fix64.FromRaw(2500);
#endif
    #endregion
    //销毁延迟时间
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
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = m_Player.m_VGo.GetComponent<Animator>();
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
        if (m_Player.m_PlayerData.m_Type == 1)
        {
            #region 普攻连击逻辑
            //普攻状态并且技能数据为空不执行
            //if (m_Player.m_IsAttack && m_Player.m_SkillNode == null)
            //    return;
            //普攻状态并且动作没有播完不执行
            if (m_Player.m_IsAttack && m_Player.m_SkillIndex != 0 && m_Player.m_IntervalTime < (Fix64)m_Player.m_SkillNode.animatorTime * m_AttackTime)
                return;
            //普攻状态并且没有播放普攻动作
            if ((m_Player.m_IsAttack && m_Player.m_SkillIndex == 0) || (!m_Player.m_IsAttack && m_Player.m_SkillIndex == 0))
            {
                m_Player.m_SkillIndex = 1;
                m_Player.m_IntervalTime = Fix64.Zero;
            }
            //普攻状态并且是第一个连击并且动作已经播放结束
            else if (m_Player.m_IsAttack && m_Player.m_SkillIndex == 1 && m_Player.m_IntervalTime >= (Fix64)m_Player.m_SkillNode.animatorTime * m_AttackTime)
            {
                m_Player.m_SkillIndex = 2;
                m_Player.m_IntervalTime = Fix64.Zero;
            }
            //普攻状态并且是第二个连击并且动作已经播放结束
            else if (m_Player.m_IsAttack && m_Player.m_SkillIndex == 2 && m_Player.m_IntervalTime >= (Fix64)m_Player.m_SkillNode.animatorTime * m_AttackTime)
            {
                m_Player.m_SkillIndex = 3;
                m_Player.m_IntervalTime = Fix64.Zero;
            }
            //普攻状态并且是第三个连击并且动作已经播放结束不执行，这儿自动走OnExit退出
            else if (m_Player.m_IsAttack && m_Player.m_SkillIndex == 3 && m_Player.m_IntervalTime >= (Fix64)m_Player.m_SkillNode.animatorTime * m_AttackTime)
            {
                m_Player.m_SkillIndex = 0;
                m_Player.m_IntervalTime = Fix64.Zero;
            }
            #endregion
        }
        else
        {
            m_Player.m_SkillIndex = 1;
            m_Player.m_IntervalTime = Fix64.Zero;
        }
        m_Player.m_SkillNode = m_Player.m_PlayerData.GetSkillNode(m_Player.m_SkillIndex);
        if (m_Player.m_SkillNode == null)
            return;
        FixVector3 pos = FixVector3.Zero;
        Player targetPlayer = m_Player.FindTarget(m_Player.m_SkillNode);
        Tower targetTower = m_Player.FindTowerTarget(m_Player.m_SkillNode);
        if (targetTower != null)
            pos = targetTower.m_Pos;
        if (targetPlayer != null)
            pos = targetPlayer.m_Pos;
        m_Player.m_IsAttack = true;
        m_Player.m_IsCalcDamage = false;
        if (pos != FixVector3.Zero)
        {
            //普通攻击自动改变朝向
            FixVector3 relativePos = pos - m_Player.m_Pos;
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_Player.m_Rotation = new FixVector3((Fix64)rotation.eulerAngles.x, (Fix64)rotation.eulerAngles.y, (Fix64)rotation.eulerAngles.z);
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
                m_Player.m_VGo.transform.rotation = rotation;
            m_Player.m_Angles = new FixVector3((Fix64)m_Player.m_VGo.transform.forward.x, (Fix64)m_Player.m_VGo.transform.forward.y, (Fix64)m_Player.m_VGo.transform.forward.z).GetNormalized();
            #endregion
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, m_Player.m_SkillIndex);
            if (m_Player.m_PlayerData.m_Type == 1)
                m_AniEffect = GameObject.Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", m_Player.m_PlayerData.m_HeroName, m_Player.m_SkillNode.spell_motion)));
            if (m_Player.m_PlayerData.m_Type == 2)
                m_AniEffect = GameObject.Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Monster", m_Player.m_PlayerData.m_HeroName, m_Player.m_SkillNode.spell_motion)));
            if (m_AniEffect == null)
                return;
            m_AniEffect.transform.parent = m_Player.m_VGo.transform;
            m_AniEffect.transform.localPosition = Vector3.zero;
            m_AniEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
            m_AniEffect.transform.localScale = Vector3.one;
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
        if (!m_Player.m_IsAttack)
            return;
        if (m_Player.m_SkillNode == null)
            return;
        m_Player.m_IntervalTime += GameData.m_FixFrameLen;
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
        m_Player.m_IsAttack = false;
        m_Player.m_IsCalcDamage = false;
        m_Player.m_SkillNode = null;
        m_Player.m_IntervalTime = Fix64.Zero;
        m_Player.m_SkillIndex = 0;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
        //if (GameData.m_GameManager != null && GameData.m_GameManager.m_LogMessage != null)
        //    GameData.m_GameManager.m_LogMessage.text += string.Format("{0}:{1},", GameData.m_GameFrame, m_Player.m_VGo.transform.position);
    }
}
