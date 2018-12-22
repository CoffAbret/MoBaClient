using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小兵数据类
/// </summary>
public class MonsterData : BaseData
{
    //小兵Id
    public int m_MonsterId;
    //小兵模型资源路径
    public string m_MonsterModelPath;
    //小兵基础属性
    public CharacterAttrNode m_MonsterAttrNode;
    //小兵技能
    public List<SkillNode> m_SkillList;
    //小兵资源名
    public string m_MonsterResourceName;
    //小兵寻路点
    public FixVector3 m_NaviPos = FixVector3.Zero;
    public MonsterData() { }
    public MonsterData(int monsterId, CampType campId, ObjectType type)
    {
        m_MonsterId = monsterId;
        m_CampId = campId;
        m_Type = type;
        m_SkillList = new List<SkillNode>();
        m_MonsterAttrNode = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType(monsterId);
        Moba3v3NaviNode naviNode = FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().FindDataByType((int)type - 1);
        m_NaviPos = campId == CampType.BLUE ? new FixVector3((Fix64)naviNode.naviPoint2.x, (Fix64)naviNode.naviPoint2.y, (Fix64)naviNode.naviPoint2.z) : new FixVector3((Fix64)naviNode.naviPoint1.x, (Fix64)naviNode.naviPoint1.y, (Fix64)naviNode.naviPoint1.z);
        if (m_MonsterAttrNode == null)
            return;
        for (int i = 0; i < m_MonsterAttrNode.skill_id.Length; i++)
        {
            SkillNode skillNode = FSDataNodeTable<MonsterSkillNode>.GetSingleton().FindDataByType(m_MonsterAttrNode.skill_id[i]);
            m_SkillList.Add(skillNode);
        }
        m_HP = m_MaxHP = m_MonsterAttrNode.hp;
        m_MonsterModelPath = m_MonsterAttrNode.modelNode.respath;
        m_MonsterResourceName = m_MonsterModelPath.Split('/')[3];
    }

    /// <summary>
    /// 获取当前技能数据
    /// </summary>
    /// <param name="skillIndex"></param>
    /// <returns></returns>
    public SkillNode GetSkillNode(int skillIndex)
    {
        for (int i = 0; i < m_SkillList.Count; i++)
        {
            if ((skillIndex - 1) == i)
                return m_SkillList[i];
        }
        return null;
    }
}
