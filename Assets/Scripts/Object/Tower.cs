using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭塔
/// </summary>
public class Tower : BaseObject
{
    //塔数据
    public TowerData m_TowerData;
    //攻击目标
    public BaseObject m_TargetObject;
    //技能间隔时间
    public Fix64 m_IntervalTime = Fix64.Zero;
    //子弹
    public TowerAttack m_TowerAttack;
    //攻击距离
    public Fix64 m_AttackDistince = Fix64.FromRaw(2500);
    //销毁延迟时间
    private Fix64 m_DestoryDelayTime = Fix64.FromRaw(1000);
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //塔攻击范围
    public GameObject m_Quan;
    //攻击特效
    public GameObject m_Attack;
    //死亡特效
    public GameObject m_Die;
#endif
    #endregion
    public Tower() { }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(TowerData towerData)
    {
        m_TowerData = new TowerData(towerData.m_CampId, towerData.m_Type, towerData.m_HP);
        m_Data = m_TowerData;
        m_TowerAttack = new TowerAttack();
        if (towerData.m_CampId == CampType.BLUE && towerData.m_Type == ObjectType.ARROW_TOWER)
            m_VGo = GameObject.Find("Tower_Blue");
        if (towerData.m_CampId == CampType.RED && towerData.m_Type == ObjectType.ARROW_TOWER)
            m_VGo = GameObject.Find("Tower_Red");
        if (towerData.m_CampId == CampType.BLUE && towerData.m_Type == ObjectType.CRYSTAL_TOWER)
            m_VGo = GameObject.Find("Camp_Blue");
        if (towerData.m_CampId == CampType.RED && towerData.m_Type == ObjectType.CRYSTAL_TOWER)
            m_VGo = GameObject.Find("Camp_Red");
        #region 显示层
        //是否执行显示层逻辑
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Quan = m_VGo.transform.Find("quan_hero").gameObject;
            m_Attack = m_VGo.transform.Find("attack0").gameObject;
            m_Die = m_VGo.transform.Find("death").gameObject;
            m_SelectedGo = m_VGo.transform.Find("Effect_targetselected").gameObject;
            m_Health = m_VGo.GetComponent<PlayerHealth>();
            m_Health.m_Health = towerData.m_HP;
            m_Pos = new FixVector3((Fix64)m_VGo.transform.position.x, (Fix64)m_VGo.transform.position.y, (Fix64)m_VGo.transform.position.z);
            MobaMiniMap.instance.AddMapIconByType(this);
        }
        #endregion
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_TowerAttack == null)
            return;
        m_TargetObject = FindTarget(m_AttackDistince);
        if (m_TargetObject == null)
            m_Quan.SetActive(false);
        else
            m_Quan.SetActive(true);

        Fix64 temp = m_IntervalTime / Fix64.One;
        if (temp > Fix64.One)
            m_IntervalTime = Fix64.Zero;

        if (m_TargetObject != null && m_IntervalTime == Fix64.Zero)
            m_TowerAttack.Create(this, m_TargetObject);

        m_TowerAttack.UpdateLogic();
        m_IntervalTime += GameData.m_FixFrameLen;
    }

    public void DestoryTowerAttack()
    {
        m_TowerAttack = null;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        GameData.m_GameManager.m_GridManager.SetWalkable(this);
        GameData.m_ObjectList.Remove(this);
        if (m_TowerAttack != null)
            m_TowerAttack.Destroy();
        m_IntervalTime = Fix64.Zero;
        m_Pos = FixVector3.Zero;
        m_TowerAttack = null;
        m_TargetObject = null;
        #region 显示层
        if (m_Die != null)
            m_Die.SetActive(true);
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject.Destroy(m_VGo, (float)m_DestoryDelayTime);
        }
        if (m_DestoryMinMapCallback != null)
            m_DestoryMinMapCallback(this);
        #endregion
        m_Die = null;
        m_Quan = null;
        m_VGo = null;
    }
}
