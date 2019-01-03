using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 角色对象
/// </summary>
public class Player : BaseObject
{
    //角色数据
    public PlayerData m_PlayerData;
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
    #region 显示层

#if IS_EXECUTE_VIEWLOGIC
    //显示对象飘血组件
    public PlayerHudText m_HudText;
#endif
    #endregion
    public Player()
    {
        m_Pos = FixVector3.Zero;
        m_Rotation = FixVector3.Zero;
        m_Angles = FixVector3.Zero;
        m_IsMove = false;
        m_IsAttack = false;
        m_IsSkill = false;
        m_IsDie = false;
        m_IsHit = false;
        m_State = null;
        m_IntervalTime = Fix64.Zero;
        m_SkillIndex = 0;
        m_SkillNode = null;
    }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(PlayerData playerData)
    {
        m_PlayerData = new PlayerData(playerData.m_RoleId, playerData.m_HeroId, playerData.m_RoleName, playerData.m_CampId, playerData.m_Type);
        m_Data = m_PlayerData;
        GameObject posGo = null;
        posGo = GameObject.Find(string.Format("203_SceneCtrl_Moba_1/Pos{0}", (int)playerData.m_CampId));
        m_Pos = (FixVector3)posGo.transform.position;
        m_Rotation = (FixVector3)(posGo.transform.rotation.eulerAngles);
        m_Angles = (FixVector3)(new Vector3(posGo.transform.forward.normalized.x, 0, posGo.transform.forward.normalized.z));
        #region 显示层
        //是否执行显示层逻辑
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject go = Resources.Load<GameObject>(playerData.m_HeroModelPath);
            m_VGo = GameObject.Instantiate(go);
            m_VGo.transform.parent = posGo.transform.parent;
            m_VGo.transform.localPosition = m_Pos.ToVector3();
            m_VGo.transform.localRotation = Quaternion.Euler(m_Rotation.ToVector3());
            m_SelectedGo = m_VGo.transform.Find("Effect_targetselected01").gameObject;
            m_SelectedGo.SetActive(false);
            m_Health = m_VGo.GetComponent<PlayerHealth>();
            m_HudText = m_VGo.GetComponent<PlayerHudText>();
            m_VGo.name = playerData.m_RoleId.ToString();
            m_Health.m_Health = playerData.m_HP;
            if (playerData.m_RoleId == GameData.m_CurrentRoleId)
            {
                GameData.m_CampId = playerData.m_CampId;
                GameData.m_CurrentPlayer = this;
                m_VGo.tag = "Player";
                GameObject cameraPosGo = GameObject.Find(string.Format("CameraPos{0}", (int)playerData.m_CampId));
                Camera.main.transform.localPosition = cameraPosGo.transform.localPosition;
                Camera.main.transform.localRotation = cameraPosGo.transform.localRotation;
                Camera.main.transform.localScale = cameraPosGo.transform.localScale;
                //显示主界面UI，关闭复活UI
                GameData.m_GameManager.m_UIManager.m_UpdateResurrectionCountdownUICallback(false);
                //更新技能图标
                GameData.m_GameManager.m_UIManager.m_UpdateBattleSkillUICallback(playerData.m_SkillList);
            }
            //关闭敌方复活UI
            if (playerData.m_RoleId != GameData.m_CurrentRoleId)
                GameData.m_GameManager.m_UIManager.m_UpdateEnemyResurrectionCountdownUICallback(false, playerData);
            MobaMiniMap.instance.AddMapIconByType(this);
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
        if (m_PlayerData == null)
            return;
        if (state is SkillState && parameter.Equals("8"))
        {
            AddHp(500);
            if (m_PlayerData.m_RoleId == GameData.m_CurrentRoleId)
                GameData.m_GameManager.m_UIManager.m_UpdateBattleSkillCDUICallback(30, 8);
        }
        else
        {
            if (m_State != null)
                m_State.OnExit();
            m_State = state;
            m_State.OnInit(this, parameter);
            m_State.OnEnter();
        }
    }

    /// <summary>
    /// 加血
    /// </summary>
    public void AddHp(int hp)
    {
        int addHp = m_PlayerData.m_HeroAttrNode.hp - m_PlayerData.m_HP;
        if (addHp <= 0)
            return;
        addHp = addHp >= hp ? hp : addHp;
        m_PlayerData.m_HP += addHp;
        m_Health.m_Health += addHp;
        if (m_PlayerData.m_RoleId == GameData.m_CurrentRoleId)
            m_HudText.PlayerHUDText.AddLocalized(string.Format("+{0}", hp), new Color(0.09F, 0.9F, 0.09F, 1), 1);
    }


    /// <summary>
    /// 遍历状态
    /// </summary>
    public override void UpdateLogic()
    {
        if (m_State == null)
            return;
        m_State.UpdateLogic();
        GameData.m_GameManager.m_UIManager.m_UpdateAddHpCallback(this);
    }

    /// <summary>
    /// 查找离玩家最近的可攻击目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public BaseObject FindTarget(SkillNode skillNode)
    {
        Fix64 preDistance = Fix64.Zero;
        BaseObject preObject = null;
        for (int i = 0; i < GameData.m_ObjectList.Count; i++)
        {
            if (GameData.m_ObjectList[i] == null || GameData.m_ObjectList[i].m_Data == null)
                continue;
            if (GameData.m_ObjectList[i].m_Data.m_CampId == m_PlayerData.m_CampId)
                continue;
            Fix64 distance = Fix64.Zero;
            if (GameData.m_ObjectList[i].m_Data.m_Type == ObjectType.PLAYER)
                distance = FixVector3.Distance(GameData.m_ObjectList[i].m_Pos, m_Pos) - Fix64.FromRaw(200);
            else if (GameData.m_ObjectList[i].m_Data.m_Type == ObjectType.MONSTER)
                distance = FixVector3.Distance(GameData.m_ObjectList[i].m_Pos, m_Pos) - Fix64.FromRaw(100);
            else if (GameData.m_ObjectList[i].m_Data.m_Type == ObjectType.ARROW_TOWER)
                distance = FixVector3.Distance(GameData.m_ObjectList[i].m_Pos, m_Pos) - Fix64.FromRaw(500);
            else if (GameData.m_ObjectList[i].m_Data.m_Type == ObjectType.CRYSTAL_TOWER)
                distance = FixVector3.Distance(GameData.m_ObjectList[i].m_Pos, m_Pos) - Fix64.FromRaw(1000);
            if ((preDistance == Fix64.Zero || preDistance > distance) && (float)distance <= skillNode.aoe_long)
            {
                preObject = GameData.m_ObjectList[i];
                preDistance = distance;
            }
        }
        return preObject;
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="playerAttack">攻击者</param>
    /// <param name="skillNode">攻击技能</param>
    public override void FallDamage(int damage)
    {
        if (m_PlayerData == null || m_PlayerData.m_HP <= 0 || damage <= 0)
            return;
        m_PlayerData.m_HP -= damage;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Health != null)
                m_Health.m_Health -= damage;
            if (m_HudText != null && m_PlayerData != null && GameData.m_CurrentPlayer != null && GameData.m_CurrentPlayer.m_PlayerData != null && m_PlayerData.m_RoleId == GameData.m_CurrentRoleId)
                m_HudText.PlayerHUDText.AddLocalized(string.Format("-{0}", damage), Color.red, 0f);
        }
        #endregion
        if (m_PlayerData.m_HP <= 0)
        {
            m_State = new DieState();
            m_State.OnInit(this);
            m_State.OnEnter();
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
            {
                if (m_PlayerData.m_RoleId == GameData.m_CurrentRoleId)
                    GameData.m_GameManager.m_UIManager.m_UpdateResurrectionCountdownUICallback(true);
                else
                    GameData.m_GameManager.m_UIManager.m_UpdateEnemyResurrectionCountdownUICallback(true, m_PlayerData);
            }
            #endregion
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public override void Destroy()
    {
        GameData.m_ObjectList.Remove(this);
        m_PlayerData = null;
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
