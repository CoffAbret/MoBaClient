using UnityEngine;

public class HeroAttrNode : CharacterAttrNode
{
    public int break_gold;
    public int grade;//英雄品级  1：白，2：绿，3：蓝，4：紫，5：橙:，6：红；
    public int break_lv;//进阶需求等级
    public long next_grade;//进阶结果
    public double[] equipment;//能穿戴的装备 以[0,0,0,0,0]的形式依次配置
    public long[,] material; //英雄突破需要的材料（id，数量）
    public HeroNode heroNode;

    public override void ParseJson(object jd)
    {
        base.ParseJson(jd);
        id = long.Parse(item["hero_id"].ToString());
        break_gold = int.Parse(item["break_gold"].ToString());

        if (FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList.ContainsKey(id))
        {
            heroNode = FSDataNodeTable<HeroNode>.GetSingleton().DataNodeList[id];
            if (heroNode != null && heroNode.released == 1)
            {
                HeroSkinNode skinNode = null;
                if (FSDataNodeTable<HeroSkinNode>.GetSingleton().DataNodeList.ContainsKey(heroNode.skinID[0]))
                    skinNode = FSDataNodeTable<HeroSkinNode>.GetSingleton().FindDataByType(heroNode.skinID[0]);
                if (skinNode != null)
                {
                    icon_name = skinNode.iconName;
                    model = skinNode.modelId;
                    modelNode = skinNode.modelNode;
                }
                skill_id = heroNode.skill_id;
                skills = new SkillNode[skill_id.Length];
                for (int i = 0; i < skills.Length; i++)
                {
                    if (FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList.ContainsKey(skill_id[i]))
                    {
                        skills[i] = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skill_id[i]];
                        if (skills[i].site != 0) skillNodeDict.Add(skills[i].site, skills[i]);
                    }
                    else
                    {
                        GameDebug.LogError("策划好好看看技能表" + skill_id[i]);
                    }

                }
                skill1_id = heroNode.skill1_id;

                grade = int.Parse(item["grade"].ToString());
                next_grade = long.Parse(item["next_grade"].ToString());
                break_lv = int.Parse(item["break_lv"].ToString());

                #region 英雄突破所需材料

                if (item.ContainsKey("break_item"))
                {
                    object[] breakMaterial = (object[])item["break_item"];
                    material = new long[breakMaterial.Length, 2];
                    if (breakMaterial.Length > 0)
                    {
                        for (int i = 0; i < breakMaterial.Length; i++)
                        {
                            int[] node = breakMaterial[i] as int[];
                            if (node != null)
                            {
                                for (int j = 0; j < node.Length; j++)
                                {
                                    material[i, j] = node[j];
                                }
                            }
                        }
                    }
                }
            }


            #endregion          
        }
    }
}

