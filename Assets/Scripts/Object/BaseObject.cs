using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject
{
    //对象数据
    public BaseData m_Data;
    //是否普攻
    public bool m_IsAttack = false;
    //是否移动
    public bool m_IsMove = false;
    //是否技能
    public bool m_IsSkill = false;
    //是否死亡
    public bool m_IsDie = false;
    //是否后仰
    public bool m_IsHit = false;
    //位置
    public FixVector3 m_Pos;
    //转向
    public FixVector3 m_Rotation;
    //朝向
    public FixVector3 m_Angles;
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //显示对象
    public GameObject m_VGo;
    //显示对象血条组件
    public PlayerHealth m_Health;
    //选中对象
    public GameObject m_SelectedGo;
    //小地图Icon
    public UISprite m_MapIcon;
    //小地图销毁回调
    public delegate void DestoryMinMapCallback(BaseObject player);
    public DestoryMinMapCallback m_DestoryMinMapCallback;
#endif
    #endregion

    /// <summary>
    /// 遍历状态
    /// </summary>
    public virtual void UpdateLogic() { }
    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void Destroy() { }
    //减血量
    public virtual void FallDamage(int damage) { }
    //查找攻击目标
    public virtual BaseObject FindTarget(Fix64 attackDistince)
    {
        Fix64 preDistance = attackDistince;
        BaseObject preObject = null;
        for (int i = 0; i < GameData.m_ObjectList.Count; i++)
        {
            if (GameData.m_ObjectList[i] == null || GameData.m_ObjectList[i].m_Data == null)
                continue;
            if (GameData.m_ObjectList[i].m_Data.m_CampId == m_Data.m_CampId)
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
            if (distance < preDistance)
            {
                preObject = GameData.m_ObjectList[i];
                preDistance = distance;
            }
        }
        return preObject;
    }
}
