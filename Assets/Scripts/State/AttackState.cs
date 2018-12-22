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
    //目标
    private FixVector3 m_TargetPos = FixVector3.Zero;
    //技能索引
    private int m_SkillIndex;
    //技能数据
    private SkillNode m_SkillNode;
    //目标
    private BaseObject m_TargetObject;

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(BaseObject baseObject, string parameter = null)
    {
        base.OnInit(baseObject, parameter);
        if (baseObject == null)
            return;
        m_SkillIndex = int.Parse(m_Parameter);
        if (baseObject is Player)
            m_SkillNode = (baseObject as Player).m_PlayerData.GetSkillNode(m_SkillIndex);
        if (baseObject is Monster)
            m_SkillNode = (baseObject as Monster).m_MonsterData.GetSkillNode(m_SkillIndex);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = baseObject.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (m_BaseObject == null || m_BaseObject.m_Data == null)
            return;
        m_BaseObject.m_IsAttack = true;
        Fix64 attackDistince = (Fix64)m_SkillNode.dist;
        m_TargetObject = m_BaseObject.FindTarget(attackDistince);
        if (m_SkillNode.is_turnround && m_TargetObject != null)
        {
            //普通攻击自动改变朝向
            FixVector3 relativePos = m_TargetObject.m_Pos - m_BaseObject.m_Pos;
            relativePos = new FixVector3(relativePos.x, Fix64.Zero, relativePos.z);
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_BaseObject.m_Rotation = new FixVector3((Fix64)rotation.eulerAngles.x, (Fix64)rotation.eulerAngles.y, (Fix64)rotation.eulerAngles.z);
            m_BaseObject.m_Angles = relativePos.GetNormalized();
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
                m_BaseObject.m_VGo.transform.rotation = rotation;
            #endregion
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, m_SkillIndex);
            #region  加载施法特效
            if (m_SkillNode.effect != null && m_SkillNode.effect.Length > 0)
            {
                for (int i = 0; i < m_SkillNode.effect.Length; i++)//加载施法特效
                {
                    if (i >= m_SkillNode.effect_start.Length || i >= m_SkillNode.effect_end.Length)
                    {
                        return;
                    }
                    int count_temp = 0;
                    Delay delay = new Delay();
                    delay.DelayDo((Fix64)m_SkillNode.effect_start[i], () =>
                    {
                        string eff = m_SkillNode.effect[count_temp];
                        if (m_BaseObject.m_Data.m_Type == ObjectType.PLAYER && !string.IsNullOrEmpty(eff))
                        {
                            GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", (m_BaseObject as Player).m_PlayerData.m_HeroResourceName, eff));
                            if (effectGo != null)
                                m_AniEffect = GameObject.Instantiate(effectGo);
                        }
                        if (m_BaseObject.m_Data.m_Type == ObjectType.MONSTER && !string.IsNullOrEmpty(eff))
                        {
                            GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Monster", (m_BaseObject as Monster).m_MonsterData.m_MonsterResourceName, eff));
                            if (effectGo != null)
                                m_AniEffect = GameObject.Instantiate(effectGo);
                        }
                        if (m_AniEffect == null)
                            return;
                        if (/*count_temp >= m_SkillNode.effect_position.Length ||*/ count_temp >= m_SkillNode.effect_positionxyz.Count)
                        {
                            Debug.LogError("     施法特效挂点数组,位置偏移数组长度不对       ");
                            return;
                        }
                        #region 后修改为挂点
                        m_AniEffect.transform.parent = m_BaseObject.m_VGo.transform;
                        #endregion
                        m_AniEffect.transform.localPosition = Vector3.zero + m_SkillNode.effect_positionxyz[count_temp];
                        m_AniEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        m_AniEffect.transform.localScale = Vector3.one;
                        Delay end_delay = new Delay();
                        end_delay.InitDestory(m_AniEffect, (Fix64)m_SkillNode.effect_end[count_temp]);
                        GameData.m_GameManager.m_DelayManager.m_DelayList.Add(end_delay);
                        count_temp++;
                    });
                    GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
                }
            }
            #endregion
        }
        #endregion
        #region 创建子弹
        if (m_SkillNode.bullet_id != null && m_SkillNode.bullet_id.Length > 0)
        {
            int count_temp = 0;
            for (int i = 0; i < m_SkillNode.bullet_id.Length; i++)
            {
                if (i >= m_SkillNode.bullet_time.Length)
                {
                    Debug.LogError("     子弹触发时间点数组长度不对       ");
                    return;
                }
                Delay delay = new Delay();
                delay.DelayDo((Fix64)m_SkillNode.bullet_time[i], () =>
                {
                    BaseBullet m_SkillState = new BaseBullet();
                    Bullet_ValueClass bullet = new Bullet_ValueClass();
                    bullet.m_BulletIndex = (Fix64)count_temp;
                    if (m_SkillNode.bullet_id != null)
                    {
                        bullet.m_BulletId = (Fix64)m_SkillNode.bullet_id[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bullet_id");
                    }
                    if (m_SkillNode.bul_target_type != null)
                    {
                        bullet.m_bul_target_type = (Fix64)m_SkillNode.bul_target_type[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bul_target_type");
                    }
                    if (m_SkillNode.bul_target_value != null)
                    {
                        //bullet.m_bul_target_value = new Fix64[m_SkillNode.bul_target_value[count_temp].Length];
                        //for (int j = 0; j < m_SkillNode.bul_target_value[count_temp].Length; j++)
                        //{
                        //    bullet.m_bul_target_value[j] = (Fix64)m_SkillNode.bul_target_value[count_temp][j];
                        //}
                        bullet.m_bul_target_value = (Fix64)m_SkillNode.bul_target_value[count_temp][0];
                    }
                    else
                    {
                        Debug.LogError("bul_target_value");
                    }
                    if (m_SkillNode.bul_target_size != null)
                    {
                        bullet.m_bul_target_size = (Fix64)m_SkillNode.bul_target_size[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bul_target_size");

                    }
                    if (m_SkillNode.bul_start != null)
                    {
                        bullet.m_bul_start = (Fix64)m_SkillNode.bul_start[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bul_start");
                    }
                    if (m_SkillNode.firing_xyz != null)
                    {
                        bullet.m_firing_xyz = (FixVector3)m_SkillNode.firing_xyz[count_temp];
                    }
                    else
                    {
                        Debug.LogError("firing_xyz");
                    }
                    bullet.m_bul_end = (Fix64)m_SkillNode.bul_end;
                    if (m_SkillNode.bul_end_xyz != null)
                    {
                        bullet.m_bul_end_xyz = (FixVector3)m_SkillNode.bul_end_xyz[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bul_end_xyz");
                    }
                    if (m_SkillNode.bul_end_angle != null)
                    {
                        bullet.m_bul_end_angle = (Fix64)m_SkillNode.bul_end_angle[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bul_end_angle");
                    }
                    if (m_SkillNode.bul_son_max != null)
                    {
                        bullet.m_bul_son_max = (Fix64)m_SkillNode.bul_son_max[count_temp];
                    }
                    else
                    {
                        Debug.LogError("bul_son_max");
                    }
                    if (m_SkillNode.max_bul != null)
                    {
                        bullet.m_max_bul = (Fix64)m_SkillNode.max_bul[count_temp];
                    }
                    else
                    {
                        Debug.LogError("max_bul");
                    }
                    switch (m_SkillNode.skill_usetype)
                    {
                        case SkillUseType.None:
                            break;
                        case SkillUseType.Direction://方向型
                            bullet.v_pos = m_BaseObject.m_Pos + (FixVector3)(m_BaseObject.m_Angles.ToVector3().normalized * 10);

                            break;
                        case SkillUseType.Point://坐标点型
                            bullet.v_pos = (FixVector3)m_TargetObject.m_Pos;

                            break;
                        case SkillUseType.Forward://直接释放型
                            bullet.v_pos = m_TargetObject.m_Pos + (FixVector3)(m_TargetObject.m_Angles.ToVector3().normalized * 10);
                            break;
                        case SkillUseType.Target://目标型
                            bullet.v_taregt.Add(m_TargetObject);
                            bullet.m_taregt = m_TargetObject;
                            break;
                        default:
                            break;
                    }
                    for (int j = 0; j < (int)bullet.m_max_bul; j++)
                    {
                        m_SkillState.CreateBullet(m_BaseObject, m_SkillNode, bullet, m_Parameter);
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
        if (m_BaseObject == null)
            return;
        if (!m_BaseObject.m_IsAttack)
            return;
        if (m_SkillNode == null)
            return;
        m_IntervalTime += GameData.m_FixFrameLen;

        //if (m_Player.m_IntervalTime >= (((Fix64)m_SkillNode.animatorTime * m_CalcDamageTime)) && !m_Player.m_IsLaunchAttack)
        //{
        //    PlayerAttack attack = new PlayerAttack();
        //    attack.Create(m_Player, m_SkillNode);
        //    GameData.m_GameManager.m_AttackManager.m_AttackList.Add(attack);
        //    m_Player.m_IsLaunchAttack = true;
        //}

        if (m_IntervalTime >= (Fix64)m_SkillNode.animatorTime)
            OnExit();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_BaseObject == null)
            return;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Animator.SetInteger(m_StateParameter, 0);
            if (m_AniEffect != null)
                GameObject.DestroyImmediate(m_AniEffect);
        }
        #endregion
        m_BaseObject.m_IsAttack = false;
        m_IntervalTime = Fix64.Zero;
        m_SkillIndex = 0;
    }
}
