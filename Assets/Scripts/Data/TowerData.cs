using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 塔数据类
/// </summary>
public class TowerData : BaseData
{
    public TowerData() { }
    public TowerData(CampType campId, ObjectType type, int hp)
    {
        m_CampId = campId;
        m_Type = type;
        m_HP = m_MaxHP = hp;
    }
}
