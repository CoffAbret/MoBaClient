using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏对象数据
/// </summary>
public class BaseData
{
    //最大血量
    public int m_MaxHP;
    //血量
    public int m_HP;
    //所属阵营(1 蓝方 2红方 3中立)
    public CampType m_CampId;
    //游戏对象类型(1 角色 2 小兵 3箭塔 4 水晶)
    public ObjectType m_Type;
}

public enum ObjectType
{
    PLAYER = 1,
    MONSTER = 2,
    ARROW_TOWER = 3,
    CRYSTAL_TOWER = 4
}

public enum CampType
{
    BLUE = 1,
    RED = 2,
    NEUTRAL = 3
}
