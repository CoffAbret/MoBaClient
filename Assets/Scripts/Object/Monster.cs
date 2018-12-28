using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 小兵对象
/// </summary>
public class Monster : BaseObject
{
    //角色数据
    public MonsterData m_MonsterData;
    //当前状态
    public BaseState m_State;
    //技能间隔时间
    public Fix64 m_IntervalTime = Fix64.Zero;
    //技能索引
    public int m_SkillIndex = 0;
    //当前技能信息
    public SkillNode m_SkillNode;
    //销毁延迟时间
    private Fix64 m_DestoryDelayTime = Fix64.FromRaw(1000);
    //角色AI
    public MonsterAI m_MonsterAI;
    public Monster()
    {
        m_Pos = FixVector3.Zero;
        m_Rotation = FixVector3.Zero;
        m_Angles = FixVector3.Zero;
        m_MonsterData = null;
        m_IsMove = false;
        m_IsAttack = false;
        m_IsSkill = false;
        m_IsDie = false;
        m_IsHit = false;
        m_State = null;
        m_IntervalTime = Fix64.Zero;
        m_SkillIndex = 0;
        m_SkillNode = null;
        m_MonsterAI = null;
    }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(MonsterData monsterData)
    {
        m_MonsterData = new MonsterData(monsterData.m_MonsterId, monsterData.m_CampId, monsterData.m_Type);
        m_Data = m_MonsterData;
        GameObject posGo = null;
        posGo = GameObject.Find(string.Format("203_SceneCtrl_Moba_1/JinZhan{0}", (int)monsterData.m_CampId));
        m_Pos = (FixVector3)posGo.transform.position;
        m_Rotation = (FixVector3)(posGo.transform.rotation.eulerAngles);
        m_Angles = (FixVector3)(new Vector3(posGo.transform.forward.normalized.x, 0, posGo.transform.forward.normalized.z));
        #region 显示层
        //是否执行显示层逻辑
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject go = Resources.Load<GameObject>(monsterData.m_MonsterModelPath);
            m_VGo = GameObject.Instantiate(go);
            m_VGo.transform.parent = posGo.transform.parent;
            m_VGo.transform.localPosition = m_Pos.ToVector3();
            m_VGo.transform.localRotation = Quaternion.Euler(m_Rotation.ToVector3());
            m_SelectedGo = m_VGo.transform.Find("Effect_targetselected01").gameObject;
            m_SelectedGo.SetActive(false);
            m_Health = m_VGo.GetComponent<PlayerHealth>();
            m_VGo.name = monsterData.m_MonsterId.ToString();
            m_Health.m_Health = monsterData.m_HP;
            MobaMiniMap.instance.AddMapIconByType(this);
        }
        #endregion
        m_MonsterAI = new MonsterAI();
        m_MonsterAI.OnInit(this);
        m_MonsterAI.OnEnter();
    }

    /// <summary>
    ///切换状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="parameter"></param>
    public void ChangeState(BaseState state, string parameter)
    {
        if (m_MonsterData == null)
            return;
        if (m_State != null)
            m_State.OnExit();
        m_State = state;
        m_State.OnInit(this, parameter);
        m_State.OnEnter();
    }


    /// <summary>
    /// 遍历状态
    /// </summary>
    public override void UpdateLogic()
    {
        if (m_MonsterAI == null)
            return;
        m_MonsterAI.UpdateLogic();
        if (m_State == null)
            return;
        m_State.UpdateLogic();
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="playerAttack">攻击者</param>
    /// <param name="skillNode">攻击技能</param>
    public override void FallDamage(int damage)
    {
        if (m_MonsterData == null || m_MonsterData.m_HP <= 0 || damage <= 0)
            return;
        m_MonsterData.m_HP -= damage;
        if (m_MonsterData.m_HP <= 0)
        {
            m_State = new DieState();
            m_State.OnInit(this);
            m_State.OnEnter();
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Health != null)
                m_Health.m_Health -= damage;
        }
        #endregion
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {
        GameData.m_ObjectList.Remove(this);
        m_MonsterData = null;
        m_IsMove = false;
        m_IsAttack = false;
        m_IsSkill = false;
        m_IsDie = false;
        m_IsHit = false;
        m_State = null;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_VGo != null)
                GameObject.Destroy(m_VGo, (float)m_DestoryDelayTime);
            m_VGo = null;
        }
        if (m_DestoryMinMapCallback != null)
            m_DestoryMinMapCallback(this);
        #endregion
    }
}
