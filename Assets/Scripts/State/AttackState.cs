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
    private Fix64 m_CalcDamageTime = Fix64.FromRaw(500);
    //普攻切换时间
    private Fix64 m_AttackTime = Fix64.FromRaw(600);
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
        GameData.m_CurrentPlayer.m_IntervalTime = Fix64.Zero;
        m_Player.m_SkillIndex = int.Parse(m_Parameter);
        m_Player.m_SkillNode = m_Player.m_PlayerData.GetSkillNode(m_Player.m_SkillIndex);
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
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        FixVector3 pos = FixVector3.Zero;
        Player targetPlayer = m_Player.FindTarget(m_Player.m_SkillNode);
        Tower targetTower = m_Player.FindTowerTarget(m_Player.m_SkillNode);
        if (targetTower != null)
        {
            m_Player.m_TargetTower = targetTower;
            if (m_Player.m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                m_Player.m_TargetTower.m_SelectedGo.SetActive(true);
            pos = m_Player.m_TargetTower.m_Pos;
        }
        if (targetPlayer != null)
        {
            m_Player.m_TargetPlayer = targetPlayer;
            if (m_Player.m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                m_Player.m_TargetPlayer.m_SelectedGo.SetActive(true);
            pos = m_Player.m_TargetPlayer.m_Pos;
        }
        m_Player.m_IsAttack = true;
        m_Player.m_IsLaunchAttack = false;
        if (pos != FixVector3.Zero)
        {
            //普通攻击自动改变朝向
            FixVector3 relativePos = pos - m_Player.m_Pos;
            relativePos = new FixVector3(relativePos.x, Fix64.Zero, relativePos.z);
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_Player.m_Rotation = new FixVector3((Fix64)rotation.eulerAngles.x, (Fix64)rotation.eulerAngles.y, (Fix64)rotation.eulerAngles.z);
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
                m_Player.m_VGo.transform.rotation = rotation;
            m_Player.m_Angles = (FixVector3)(new Vector3(m_Player.m_VGo.transform.forward.normalized.x, 0, m_Player.m_VGo.transform.forward.normalized.z));
            #endregion
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, m_Player.m_SkillIndex);
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
        if (m_Player.m_IntervalTime == (GameData.m_FixFrameLen * (Fix64)5))
        {
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
            {
                if (m_Player.m_PlayerData.m_Type == 1)
                {
                    GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", m_Player.m_PlayerData.m_HeroName, m_Player.m_SkillNode.spell_motion));
                    if (effectGo != null)
                        m_AniEffect = GameObject.Instantiate(effectGo);
                }
                if (m_Player.m_PlayerData.m_Type == 2 && !string.IsNullOrEmpty(m_Player.m_SkillNode.spell_motion))
                {
                    GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Monster", m_Player.m_PlayerData.m_HeroName, m_Player.m_SkillNode.spell_motion));
                    if (effectGo != null)
                        m_AniEffect = GameObject.Instantiate(effectGo);
                }
                if (m_AniEffect == null)
                    return;
                m_AniEffect.transform.parent = m_Player.m_VGo.transform;
                m_AniEffect.transform.localPosition = Vector3.zero;
                m_AniEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
                m_AniEffect.transform.localScale = Vector3.one;
            }
            #endregion
        }
        if (m_Player.m_IntervalTime == (GameData.m_FixFrameLen * (Fix64)10))
        {
            PlayerAttack attack = new PlayerAttack();
            attack.Create(m_Player, m_Player.m_SkillNode);
            GameData.m_GameManager.m_AttackManager.m_AttackList.Add(attack);
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
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, 0);
            if (m_AniEffect != null)
                GameObject.DestroyImmediate(m_AniEffect);
        }
        #endregion
        m_Player.m_IsAttack = false;
        m_Player.m_IsLaunchAttack = false;
        m_Player.m_SkillNode = null;
        m_Player.m_IntervalTime = Fix64.Zero;
        m_Player.m_SkillIndex = 0;
    }
}
