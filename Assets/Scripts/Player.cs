using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 显示层对象
/// </summary>
public class Player
{
    #region 逻辑层
    //位置
    public FixVector3 m_Pos;
    //转向
    public FixVector3 m_Rotation;
    //缩放
    public FixVector3 m_Scale;
    //攻击目标
    public Player m_TargetPlayer;
    //攻击目标
    public Tower m_TargetTower;
    //显示对象角度
    public FixVector3 m_Angles;
    //对象数据
    public CharData m_CharData;
    //角色索引
    public int m_PlayerIndex = 0;
    //是否主角
    public bool m_IsMe = false;
    //是否移动
    public bool m_IsMove = false;
    //是否技能移动
    public bool m_IsSkillMove = false;
    //是否普攻
    public bool m_IsAttack = false;
    //是否技能
    public bool m_IsSkill = false;
    //是否计算伤害
    public bool m_IsCalcDamage = false;
    //是否播放特效
    public bool m_IsPlayEffect = false;
    //是否死亡
    public bool m_IsDie = false;
    //是否后仰
    public bool m_IsHit = false;
    //上一次状态
    public BaseState m_PreState;
    //当前状态
    public BaseState m_State;
    //技能间隔时间
    public Fix64 m_IntervalTime = Fix64.Zero;
    //移动速度
    public Fix64 m_Speed = Fix64.FromRaw(100);
    //技能移动速度
    public Fix64 m_SkillSpeed = Fix64.FromRaw(500);
    //旋转速度
    public Fix64 m_RotationSpeed = Fix64.One;
    //技能索引
    public int m_SkillIndex = 0;
    //当前技能信息
    public SkillNode m_SkillNode;
    //销毁延迟时间
    private Fix64 m_DestoryDelayTime = Fix64.FromRaw(5000);
    #endregion
    #region 显示层

#if IS_EXECUTE_VIEWLOGIC
    //显示对象
    public GameObject m_VGo;
    //显示对象血条组件
    public PlayerHealth m_Health;
    //显示对象飘血组件
    public PlayerHudText m_HudText;
    //选中对象
    public GameObject m_SelectedGo;
#endif
    #endregion
    public Player() { }
    public Player(bool isMe) { m_IsMe = isMe; }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(CharData charData)
    {
        #region 逻辑层
        m_CharData = new CharData(charData.m_Id, charData.m_HeroId, charData.m_Name, charData.m_PlayerIndex, charData.m_CampId);
        GameObject posGo = GameObject.Find(string.Format("Pos{0}", m_CharData.m_PlayerIndex));
        m_Pos = new FixVector3((Fix64)posGo.transform.localPosition.x, (Fix64)posGo.transform.localPosition.y, (Fix64)posGo.transform.localPosition.z);
        m_Rotation = new FixVector3((Fix64)posGo.transform.localRotation.eulerAngles.x, (Fix64)posGo.transform.localRotation.eulerAngles.y, (Fix64)posGo.transform.localRotation.eulerAngles.z);
        m_Scale = new FixVector3((Fix64)posGo.transform.localScale.x, (Fix64)posGo.transform.localScale.y, (Fix64)posGo.transform.localScale.z);
        m_Angles = new FixVector3((Fix64)posGo.transform.forward.normalized.x, (Fix64)posGo.transform.forward.normalized.y, (Fix64)posGo.transform.forward.normalized.z);
        #endregion
        #region 显示层
        //是否执行显示层逻辑
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject go = Resources.Load<GameObject>(m_CharData.m_ModelPath);
            m_VGo = GameObject.Instantiate(go);
            m_VGo.transform.localPosition = m_Pos.ToVector3();
            m_VGo.transform.localRotation = Quaternion.Euler(m_Rotation.ToVector3());
            m_VGo.transform.localScale = m_Scale.ToVector3();
            m_SelectedGo = m_VGo.transform.Find("Effect_targetselected01").gameObject;
            m_SelectedGo.SetActive(false);
            m_Health = m_VGo.GetComponent<PlayerHealth>();
            m_HudText = m_VGo.GetComponent<PlayerHudText>();
            m_VGo.name = charData.m_Id.ToString();
            m_Health.m_Health = m_CharData.m_HP;
            if (m_IsMe)
            {
                m_VGo.tag = "Player";
                GameObject cameraPosGo = GameObject.Find(string.Format("CameraPos{0}", m_CharData.m_CampId));
                Camera.main.transform.localPosition = cameraPosGo.transform.localPosition;
                Camera.main.transform.localRotation = cameraPosGo.transform.localRotation;
                Camera.main.transform.localScale = cameraPosGo.transform.localScale;
            }
        }
        #endregion
    }

    /// <summary>
    ///切换状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="parameter"></param>
    public void ChangeState(BaseState state, string parameter)
    {
        if (m_CharData == null)
            return;
        if (m_State != null && !(m_State is AttackState))
        {
            m_PreState = m_State;
            m_State.OnExit();
        }
        m_State = state;
        m_State.OnInit(this, parameter);
        m_State.OnEnter();
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_State == null)
            return;
        m_State.UpdateLogic();
    }

    /// <summary>
    /// 查找目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public Player FindTarget(SkillNode skillNode)
    {
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i].m_CharData.m_CampId == m_CharData.m_CampId)
                continue;
            Fix64 distance = FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos);
            if (distance <= (Fix64)skillNode.dist)
            {
                if (m_TargetPlayer != null && m_TargetPlayer.m_SelectedGo != null)
                    m_TargetPlayer.m_SelectedGo.SetActive(false);
                m_TargetPlayer = GameData.m_PlayerList[i];
                if (m_TargetPlayer != null && m_TargetPlayer.m_SelectedGo != null)
                    m_TargetPlayer.m_SelectedGo.SetActive(true);
                return GameData.m_PlayerList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 查找目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public Tower FindTowerTarget(SkillNode skillNode)
    {
        for (int i = 0; i < GameData.m_TowerList.Count; i++)
        {
            if (GameData.m_TowerList[i].m_CampId == m_CharData.m_CampId)
                continue;
            Fix64 distance = FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos);
            if (distance <= (Fix64)skillNode.dist)
            {
                if (m_TargetTower != null && m_TargetTower.m_SelectedGo != null)
                    m_TargetTower.m_SelectedGo.SetActive(false);
                m_TargetTower = GameData.m_TowerList[i];
                if (m_TargetTower != null && m_TargetTower.m_SelectedGo != null)
                    m_TargetTower.m_SelectedGo.SetActive(true);
                return GameData.m_TowerList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 判断击中目标
    /// </summary>
    public void CalcDamage(SkillNode skillNode)
    {
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i].m_CharData.m_CampId == m_CharData.m_CampId)
                continue;
            if (GameData.m_PlayerList[i].m_CharData.m_Id == m_CharData.m_Id)
                continue;
            //玩家与敌人的方向向量
            FixVector3 targetV3 = GameData.m_PlayerList[i].m_Pos - m_Pos;
            //求玩家正前方、玩家与敌人方向两个向量的夹角
            //这地方求夹角将来要使用定点数或者其他方法换掉，暂时使用Vector3类型
            Fix64 angle = (Fix64)Vector3.Angle(m_VGo.transform.forward, targetV3.ToVector3());
            Fix64 distance = FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos);
            if (angle <= (Fix64)skillNode.angle / 2 && distance <= (Fix64)skillNode.dist)
            {
                int damage = 0;
                if (skillNode != null && skillNode.base_num1 != null && skillNode.base_num1.Length > 0)
                    damage = (int)((m_CharData.m_HeroAttrNode.attack * 15 + skillNode.base_num1[0]) - m_CharData.m_HeroAttrNode.armor);
                else
                    damage = (int)((m_CharData.m_HeroAttrNode.attack * 15) - m_CharData.m_HeroAttrNode.armor);
                if (skillNode.skill_id == 301001006)
                {
                    m_State = new HitState();
                    m_State.OnInit(this);
                    m_State.OnEnter();
                }
                GameData.m_PlayerList[i].FallDamage(damage);

            }
        }

        for (int i = 0; i < GameData.m_TowerList.Count; i++)
        {
            if (GameData.m_TowerList[i].m_CampId == m_CharData.m_CampId)
                continue;
            //玩家与敌人的方向向量
            FixVector3 targetV3 = GameData.m_TowerList[i].m_Pos - m_Pos;
            //求玩家正前方、玩家与敌人方向两个向量的夹角
            //这地方求夹角将来要使用定点数或者其他方法换掉，暂时使用Vector3类型
            Fix64 angle = (Fix64)Vector3.Angle(m_VGo.transform.forward, targetV3.ToVector3());
            Fix64 distance = FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos);
            if (angle <= (Fix64)skillNode.angle / 2 && distance <= (Fix64)skillNode.dist)
            {
                int damage = 0;
                if (skillNode != null && skillNode.base_num1 != null && skillNode.base_num1.Length > 0)
                    damage = (int)((m_CharData.m_HeroAttrNode.attack * 15 + skillNode.base_num1[0]) - m_CharData.m_HeroAttrNode.armor);
                else
                    damage = (int)((m_CharData.m_HeroAttrNode.attack * 15) - m_CharData.m_HeroAttrNode.armor);
                GameData.m_TowerList[i].FallDamage(damage);
            }
        }
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="playerAttack">攻击者</param>
    /// <param name="skillNode">攻击技能</param>
    public void FallDamage(int damage)
    {
        m_CharData.m_HP -= damage;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Health.m_Health -= damage;
            m_HudText.PlayerHUDText.gameObject.SetActive(true);
            m_HudText.gameObject.SetActive(true);
            m_HudText.PlayerHUDText.Add(-damage, Color.red, 0f);
        }
        #endregion
        if (m_CharData.m_HP <= 0)
        {
            m_State = new DieState();
            m_State.OnInit(this);
            m_State.OnEnter();
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        GameData.m_PlayerList.Remove(this);
        if (m_TargetPlayer != null && m_TargetPlayer.m_SelectedGo != null)
            m_TargetPlayer.m_SelectedGo.SetActive(false);
        m_CharData = null;
        m_IsMe = false;
        m_IsMove = false;
        m_IsAttack = false;
        m_IsSkill = false;
        m_IsCalcDamage = false;
        m_IsDie = true;
        m_IsHit = false;
        m_State = null;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_VGo != null)
                GameObject.Destroy(m_VGo, (float)m_DestoryDelayTime);
            m_VGo = null;
        }
        #endregion
    }
}
