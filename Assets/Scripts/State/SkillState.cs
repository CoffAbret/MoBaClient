using System.Collections;
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
    private Fix64 m_CalcDamageTime = Fix64.FromRaw(600);
    //播放特效时间
    private Fix64 m_PlayEffectTime = Fix64.FromRaw(60);
    //计算目标位置与当前位置的间隔距离
    FixVector3 FramePos;
#endif
    #endregion
    //普攻段数
    private int m_AttackSegments = 3;
    //目标
    private FixVector3 m_TargetPos = FixVector3.Zero;
    //创建子弹当前数量
    private int count_temp = 0;
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(Player player, string parameter = null)
    {
        base.OnInit(player, parameter);
        if (m_Player == null || string.IsNullOrEmpty(parameter))
            return;
        if (m_Parameter != null && m_Parameter.Contains("#"))
        {
            m_Player.m_SkillIndex = int.Parse(parameter.Split('#')[0]);
            float x = float.Parse(parameter.Split('#')[1]);
            float y = float.Parse(parameter.Split('#')[2]);
            float z = float.Parse(parameter.Split('#')[3]);
            FixVector3 pos = new FixVector3((Fix64)x, (Fix64)y, (Fix64)z);
            FramePos = new FixVector3(pos.GetNormalized().x, (Fix64)y, pos.GetNormalized().z);
        }
        else
        {
            m_Player.m_SkillIndex = int.Parse(m_Parameter);
        }
        m_Player.m_SkillNode = m_Player.m_PlayerData.GetSkillNode(m_Player.m_SkillIndex);
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
        Player targetPlayer = m_Player.FindTarget(m_Player.m_SkillNode);
        Tower targetTower = m_Player.FindTowerTarget(m_Player.m_SkillNode);

        if (targetTower != null)
        {
            if (m_Player.m_TargetTower != null && m_Player.m_TargetTower.m_SelectedGo != null && m_Player.m_TargetTower != targetTower)
                m_Player.m_TargetTower.m_SelectedGo.SetActive(false);
            m_Player.m_TargetTower = targetTower;
            if (m_Player.m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                m_Player.m_TargetTower.m_SelectedGo.SetActive(true);
            m_TargetPos = m_Player.m_TargetTower.m_Pos;
        }
        if (targetPlayer != null)
        {
            if (m_Player.m_TargetPlayer != null && m_Player.m_TargetPlayer.m_SelectedGo != null && m_Player.m_TargetPlayer != targetPlayer)
                m_Player.m_TargetPlayer.m_SelectedGo.SetActive(false);
            m_Player.m_TargetPlayer = targetPlayer;
            if (m_Player.m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                m_Player.m_TargetPlayer.m_SelectedGo.SetActive(true);
            m_TargetPos = m_Player.m_TargetPlayer.m_Pos;
        }

        if (m_Player.m_SkillNode.is_turnround && m_TargetPos != FixVector3.Zero)
        {
            FixVector3 relativePos = m_TargetPos - m_Player.m_Pos;
            relativePos = new FixVector3(relativePos.x, Fix64.Zero, relativePos.z);
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_Player.m_Rotation = new FixVector3((Fix64)rotation.eulerAngles.x, (Fix64)rotation.eulerAngles.y, (Fix64)rotation.eulerAngles.z);
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
                m_Player.m_VGo.transform.rotation = rotation;
            m_Player.m_Angles = (FixVector3)(new Vector3(m_Player.m_VGo.transform.forward.normalized.x, 0, m_Player.m_VGo.transform.forward.normalized.z));
            #endregion
        }
        m_Player.m_IsSkill = true;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, m_Player.m_SkillIndex);
            if (m_Player.m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                GameData.m_GameManager.m_UIManager.m_UpdateSkillCDUICallback((int)m_Player.m_SkillNode.cooling, m_Player.m_SkillIndex);
            #region  加载施法特效
            if (m_Player.m_SkillNode.effect != null && m_Player.m_SkillNode.effect.Length > 0)
            {
                for (int i = 0; i < m_Player.m_SkillNode.effect.Length; i++)//加载施法特效
                {
                    if (i >= m_Player.m_SkillNode.effect_start.Length || i >= m_Player.m_SkillNode.effect_end.Length)
                    {
                        //Debug.LogError("     施法特效开始时间，结束时间数组长度不对       ");
                        return;
                    }
                    int count_temp = 0;
                    Delay delay = new Delay();
                    delay.DelayDo((Fix64)m_Player.m_SkillNode.effect_start[i], () =>
                    {
                        string eff = m_Player.m_SkillNode.effect[count_temp];
                        if (m_Player.m_PlayerData.m_Type == 1 && !string.IsNullOrEmpty(eff))
                        {
                            GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", m_Player.m_PlayerData.m_HeroName, eff));
                            if (effectGo != null)
                                m_AniEffect = GameObject.Instantiate(effectGo);
                        }
                        if (m_Player.m_PlayerData.m_Type == 2 && !string.IsNullOrEmpty(eff))
                        {
                            GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Monster", m_Player.m_PlayerData.m_HeroName, eff));
                            if (effectGo != null)
                                m_AniEffect = GameObject.Instantiate(effectGo);
                        }
                        if (m_AniEffect == null)
                            return;
                        if (/*count_temp >= m_Player.m_SkillNode.effect_position.Length ||*/ count_temp >= m_Player.m_SkillNode.effect_positionxyz.Count)
                        {
                            //Debug.LogError("     施法特效挂点数组,位置偏移数组长度不对       ");
                            return;
                        }
                        #region 后修改为挂点
                        m_AniEffect.transform.parent = m_Player.m_VGo.transform;
                        #endregion
                        m_AniEffect.transform.localPosition = Vector3.zero + m_Player.m_SkillNode.effect_positionxyz[count_temp];
                        m_AniEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        m_AniEffect.transform.localScale = Vector3.one;
                        Delay end_delay = new Delay();
                        end_delay.InitDestory(m_AniEffect, (Fix64)m_Player.m_SkillNode.effect_end[count_temp]);
                        GameData.m_GameManager.m_DelayManager.m_DelayList.Add(end_delay);
                        count_temp++;
                    });
                    GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
                }
            }
            #endregion
        }
        #endregion
        //创建子弹
        #region
        if (m_Player.m_SkillNode.bullet_id != null && m_Player.m_SkillNode.bullet_id.Length > 0)
        {
            for (int i = 0; i < m_Player.m_SkillNode.bullet_id.Length; i++)
            {
                if (i >= m_Player.m_SkillNode.bullet_time.Length)
                {
                    //Debug.LogError("     子弹触发时间点数组长度不对       ");
                    return;
                }
                count_temp = 0;
                Delay delay = new Delay();
                delay.DelayDo((Fix64)m_Player.m_SkillNode.bullet_time[i], () =>
                {
                    BaseBullet m_SkillState = new BaseBullet();
                    Bullet_ValueClass bullet = new Bullet_ValueClass();
                    if (m_Player.m_SkillNode.bullet_id != null)
                    {
                        bullet.m_BulletId = (Fix64)m_Player.m_SkillNode.bullet_id[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bullet_id");
                    }
                    if (m_Player.m_SkillNode.bul_target_type != null)
                    {
                        bullet.m_bul_target_type = (Fix64)m_Player.m_SkillNode.bul_target_type[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bul_target_type");
                    }
                    if (m_Player.m_SkillNode.bul_target_value != null)
                    {
                        //bullet.m_bul_target_value = new Fix64[m_Player.m_SkillNode.bul_target_value[count_temp].Length];
                        //for (int j = 0; j < m_Player.m_SkillNode.bul_target_value[count_temp].Length; j++)
                        //{
                        //    bullet.m_bul_target_value[j] = (Fix64)m_Player.m_SkillNode.bul_target_value[count_temp][j];
                        //}
                        bullet.m_bul_target_value = (Fix64)m_Player.m_SkillNode.bul_target_value[count_temp][0];

                    }
                    else
                    {
                        //Debug.LogError("bul_target_value");
                    }
                    if (m_Player.m_SkillNode.bul_target_size != null)
                    {
                        bullet.m_bul_target_size = (Fix64)m_Player.m_SkillNode.bul_target_size[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bul_target_size");

                    }
                    if (m_Player.m_SkillNode.bul_start != null)
                    {
                        bullet.m_bul_start = (Fix64)m_Player.m_SkillNode.bul_start[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bul_start");
                    }
                    if (m_Player.m_SkillNode.firing_xyz != null)
                    {
                        bullet.m_firing_xyz = (FixVector3)m_Player.m_SkillNode.firing_xyz[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("firing_xyz");
                    }
                    bullet.m_bul_end = (Fix64)m_Player.m_SkillNode.bul_end;
                    if (m_Player.m_SkillNode.bul_end_xyz != null)
                    {
                        bullet.m_bul_end_xyz = (FixVector3)m_Player.m_SkillNode.bul_end_xyz[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bul_end_xyz");
                    }
                    if (m_Player.m_SkillNode.bul_end_angle != null)
                    {
                        bullet.m_bul_end_angle = (Fix64)m_Player.m_SkillNode.bul_end_angle[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bul_end_angle");
                    }
                    if (m_Player.m_SkillNode.bul_son_max != null)
                    {
                        bullet.m_bul_son_max = (Fix64)m_Player.m_SkillNode.bul_son_max[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("bul_son_max");
                    }
                    if (m_Player.m_SkillNode.max_bul != null)
                    {
                        bullet.m_max_bul = (Fix64)m_Player.m_SkillNode.max_bul[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("m_max_bul");
                    }
                    switch (m_Player.m_SkillNode.skill_usetype)
                    {
                        case SkillUseType.None:
                            break;
                        case SkillUseType.Direction://方向型
                            bullet.v_pos = m_Player.m_Pos + (FixVector3)(m_Player.m_Angles.ToVector3().normalized * 10);

                            break;
                        case SkillUseType.Point://坐标点型
                            bullet.v_pos = (FixVector3)m_Player.m_TargetPlayer.m_Pos;

                            break;
                        case SkillUseType.Forward://直接释放型
                            bullet.v_pos = m_Player.m_Pos + (FixVector3)(m_Player.m_Angles.ToVector3().normalized * 10);
                            break;
                        case SkillUseType.Target://目标型
                            bullet.v_taregt.Add(m_Player.m_TargetPlayer);
                            bullet.m_taregt = m_Player.m_TargetPlayer;
                            break;
                        default:
                            break;
                    }
                    for (int j = 0; j < (int)bullet.m_max_bul; j++)
                    {
                        m_SkillState.CreateBullet(m_Player, bullet, m_Parameter);
                        m_SkillState.OnEnter();
                        GameData.m_GameManager.m_BulletManager.m_AttackList.Add(m_SkillState);
                    }
                    count_temp++;
                });
                GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
            }
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
        //if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill || m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill2)
        //{
        //    //技能位移直接设置方向即可
        //    FixVector3 pos = FixVector3.Zero;
        //    if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill)
        //    {
        //        pos = m_Player.m_Pos + ((Fix64)m_Player.m_SkillNode.flight_speed * m_Player.m_Angles * GameData.m_FixFrameLen);
        //    }
        //    else if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill2)
        //    {
        //        m_Player.m_VGo.transform.LookAt(new Vector3(5, 4.8f, 5));
        //        pos = m_Player.m_Pos + FramePos * (Fix64)m_Player.m_SkillNode.flight_speed * GameData.m_FixFrameLen;
        //    }
        //    Vector2 gridPos = GameData.m_GameManager.m_GridManager.MapPosToGrid(pos.ToVector3());
        //    bool IsSkillMove = false;
        //    bool IsWalk = GameData.m_GameManager.m_GridManager.GetWalkable(gridPos);
        //    if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill)
        //    {
        //        IsSkillMove = IsWalk;
        //    }
        //    else
        //    {
        //        bool isTargetPos = FixVector3.Distance(m_Player.m_Pos, m_TargetPos) > GameData.m_FixFrameLen * (Fix64)20;
        //        IsSkillMove = IsWalk && isTargetPos;
        //    }
        //    if (IsSkillMove)
        //    {
        //        m_Player.m_Pos = pos;
        //        #region 显示层
        //        if (GameData.m_IsExecuteViewLogic)
        //        {
        //            m_Player.m_VGo.transform.position = m_Player.m_Pos.ToVector3();
        //        }
        //        #endregion
        //    }
        //}
        if (!m_Player.m_IsSkill)
            return;
        //if (m_Player.m_IntervalTime == (GameData.m_FixFrameLen * (Fix64)5))
        //{
        //    #region 显示层
        //    if (GameData.m_IsExecuteViewLogic)
        //    {
        //        GameObject effecGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", m_Player.m_PlayerData.m_HeroName, m_Player.m_SkillNode.spell_motion));
        //        if (effecGo == null)
        //            return;
        //        m_AniEffect = GameObject.Instantiate(effecGo);
        //        //带位移的技能需要特效也跟着角色位置移动
        //        if (m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill || m_Player.m_SkillNode.skill_type == SkillCastType.FrontSprintSkill2)
        //        {
        //            m_AniEffect.transform.parent = m_Player.m_VGo.transform;
        //            m_AniEffect.transform.localPosition = Vector3.zero;
        //            m_AniEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //        }
        //        else
        //        {
        //            m_AniEffect.transform.localPosition = m_Player.m_VGo.transform.localPosition;
        //            m_AniEffect.transform.localRotation = m_Player.m_VGo.transform.localRotation;
        //        }
        //        m_AniEffect.transform.localScale = Vector3.one;
        //        m_AniEffect.SetActive(true);
        //        if (m_Player.m_SkillNode.skill_type == SkillCastType.CenterSkill)
        //        {
        //            Delay delay = new Delay();
        //            delay.InitDestory(m_AniEffect, (Fix64)m_Player.m_SkillNode.efficiency_time);
        //            GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
        //        }
        //    }
        //    #endregion
        //}
        if (!m_Player.m_IsSkill)
            return;
        //if (m_Player.m_IntervalTime >= (((Fix64)m_Player.m_SkillNode.animatorTime * m_CalcDamageTime)) && !m_Player.m_IsLaunchAttack)
        //{
        //    PlayerAttack attack = new PlayerAttack();
        //    attack.Create(m_Player, m_Player.m_SkillNode);
        //    GameData.m_GameManager.m_AttackManager.m_AttackList.Add(attack);
        //    m_Player.m_IsLaunchAttack = true;
        //}
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
            if (m_Player.m_SkillNode != null && m_Player.m_SkillNode.skill_type != SkillCastType.CenterSkill && m_AniEffect != null)
            {
                GameObject.DestroyImmediate(m_AniEffect);
            }
        }
        #endregion
        m_Player.m_IsSkill = false;
        m_Player.m_SkillIndex = 0;
        m_Player.m_IntervalTime = Fix64.Zero;
    }
}
