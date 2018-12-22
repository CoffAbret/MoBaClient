using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色数据类
/// </summary>
public class PlayerData : BaseData
{
    //角色ID
    public int m_RoleId;
    //角色名称
    public string m_RoleName;
    //角色英雄ID
    public int m_HeroId;
    //角色英雄资源名称
    public string m_HeroResourceName;
    //角色英雄模型路径
    public string m_HeroModelPath;
    //角色英雄基础属性
    public CharacterAttrNode m_HeroAttrNode;
    //角色英雄技能
    public List<SkillNode> m_SkillList;
    public PlayerData() { }
    public PlayerData(int id, int heroId, string name, CampType campId, ObjectType type)
    {
        m_RoleId = id;
        m_HeroId = heroId;
        m_RoleName = name;
        m_CampId = campId;
        m_Type = type;
        m_SkillList = new List<SkillNode>();
        //获取英雄品质为1的基础属性
        m_HeroAttrNode = FSDataNodeTable<HeroAttrNode>.GetSingleton().FindDataByType(heroId + 1);
        if (m_HeroAttrNode == null)
            return;
        for (int i = 0; i < m_HeroAttrNode.skill_id.Length; i++)
        {
            SkillNode skillNode = FSDataNodeTable<SkillNode>.GetSingleton().FindDataByType(m_HeroAttrNode.skill_id[i]);
            m_SkillList.Add(skillNode);
        }
        m_HP = m_MaxHP = m_HeroAttrNode.hp;
        m_HeroModelPath = m_HeroAttrNode.modelNode.respath;
        m_HeroResourceName = m_HeroModelPath.Split('/')[3];
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
