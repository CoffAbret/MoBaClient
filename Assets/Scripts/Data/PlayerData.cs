using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色数据类
/// </summary>
public class PlayerData
{
    //对象唯一ID
    public int m_Id;
    //对象名称
    public string m_Name;
    //当前英雄ID
    public int m_HeroId;
    //所属阵营
    public int m_CampId;
    //当前血量
    public int m_HP;
    //当前对象模型路径
    public string m_ModelPath;
    //角色类型(1 角色 2 小兵)
    public int m_Type;
    //索引
    public int m_Index;
    //寻路点
    public FixVector3 m_NaviPos = FixVector3.Zero;
    //当前英雄基础属性
    public CharacterAttrNode m_HeroAttrNode;
    //当前英雄技能
    public List<SkillNode> m_SkillList;
    public string m_HeroName;
    public PlayerData() { }
    public PlayerData(int id, int heroId, string name, int campId, int type)
    {
        m_Id = id;
        m_HeroId = heroId;
        m_Name = name;
        m_CampId = campId;
        m_Type = type;
        m_SkillList = new List<SkillNode>();
        //获取英雄品质为1的基础属性
        if (type == 1)
            m_HeroAttrNode = FSDataNodeTable<HeroAttrNode>.GetSingleton().FindDataByType(heroId + 1);
        //获取小兵基础属性
        else
        {
            m_HeroAttrNode = FSDataNodeTable<MonsterAttrNode>.GetSingleton().FindDataByType(heroId);
            Moba3v3NaviNode naviNode = FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().FindDataByType(type - 1);
            m_NaviPos = campId == 1 ? new FixVector3((Fix64)naviNode.naviPoint2.x, (Fix64)naviNode.naviPoint2.y, (Fix64)naviNode.naviPoint2.z) : new FixVector3((Fix64)naviNode.naviPoint1.x, (Fix64)naviNode.naviPoint1.y, (Fix64)naviNode.naviPoint1.z);

        }
        if (m_HeroAttrNode == null)
            return;
        for (int i = 0; i < m_HeroAttrNode.skill_id.Length; i++)
        {
            SkillNode skillNode = null;
            if (type == 1)
                skillNode = FSDataNodeTable<SkillNode>.GetSingleton().FindDataByType(m_HeroAttrNode.skill_id[i]);
            else
                skillNode = FSDataNodeTable<MonsterSkillNode>.GetSingleton().FindDataByType(m_HeroAttrNode.skill_id[i]);
            m_SkillList.Add(skillNode);
        }
        m_HP = m_HeroAttrNode.hp;
        m_ModelPath = m_HeroAttrNode.modelNode.respath;
        m_HeroName = m_ModelPath.Split('/')[3];
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
