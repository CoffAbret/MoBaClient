using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据类
/// </summary>
public class CharData
{
    //角色ID
    public int m_Id;
    //角色名称
    public string m_Name;
    //所属阵营
    public int m_CampId;
    //当前英雄ID
    public int m_HeroId;
    //角色索引
    public int m_PlayerIndex;
    //当前血量
    public int m_HP;
    //当前英雄模型路径
    public string m_ModelPath;
    //当前英雄基础属性
    public HeroAttrNode m_HeroAttrNode;
    //当前英雄技能
    public IList<SkillNode> m_SkillList;
    public string m_HeroName;
    public CharData() { }
    public CharData(int id, int heroId, string name,int playerIndex,int campId)
    {
        m_Id = id;
        m_HeroId = heroId;
        m_Name = name;
        m_PlayerIndex = playerIndex;
        m_CampId = campId;
        m_SkillList = new List<SkillNode>();
        //获取英雄品质为1的基础属性
        m_HeroAttrNode = FSDataNodeTable<HeroAttrNode>.GetSingleton().FindDataByType(heroId + 1);
        m_HP = m_HeroAttrNode.hp;

        HeroNode heroNode = FSDataNodeTable<HeroNode>.GetSingleton().FindDataByType(heroId);
        for (int i = 0; i < heroNode.skill_id.Length; i++)
        {
            SkillNode skillNode = FSDataNodeTable<SkillNode>.GetSingleton().FindDataByType(heroNode.skill_id[i]);
            m_SkillList.Add(skillNode);
        }

        HeroSkinNode heroSkinNode = FSDataNodeTable<HeroSkinNode>.GetSingleton().FindDataByType(heroNode.skinID[0]);
        ModelNode modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(heroSkinNode.modelId);
        m_ModelPath = modelNode.respath;
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
