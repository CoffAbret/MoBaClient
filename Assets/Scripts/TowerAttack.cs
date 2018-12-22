using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭塔子弹
/// </summary>
public class TowerAttack
{
    //位置
    public FixVector3 m_Pos = FixVector3.Zero;
    //朝向
    public FixVector3 m_Angle = FixVector3.Zero;
    //攻击目标
    public BaseObject m_TargetObject;
    //攻击距离
    public Fix64 m_AttackDistince = Fix64.FromRaw(200);
    //攻击间隔
    public Fix64 m_AttackSpeed = Fix64.FromRaw(100);
    //攻击特效
    public GameObject m_Attack;
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(Tower tower, BaseObject targetObject)
    {
        m_TargetObject = targetObject;
        m_Pos = tower.m_Pos;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            m_Attack = tower.m_VGo.transform.Find("attack0").gameObject;
            m_Attack.SetActive(true);
        }
        #endregion
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        Fix64 distince = FixVector3.Distance(m_TargetObject.m_Pos, m_Pos);
        if (distince < m_AttackDistince)
        {
            m_TargetObject.FallDamage(50);
            Destroy();
        }
        else
        {
            //普通攻击子弹自动改变朝向
            FixVector3 relativePos = m_TargetObject.m_Pos - m_Pos;
            Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
            m_Angle = relativePos.GetNormalized();
            m_Pos += m_Angle * m_AttackSpeed;
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
            {
                if (m_Attack != null)
                {
                    m_Attack.transform.rotation = rotation;
                    m_Attack.transform.position += m_Pos.ToVector3();
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        m_Pos = FixVector3.Zero;
        m_TargetObject = null;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Attack != null)
            {
                m_Attack.SetActive(false);
                m_Attack.transform.localPosition = new Vector3(0, 1, 0);
                m_Attack.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }
        #endregion
    }
}
