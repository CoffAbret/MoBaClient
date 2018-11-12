using UnityEngine;

public class MonsterAttrNode : CharacterAttrNode
{
    public float lv_hp;
    public float lv_attack;
    public float lv_magic;
    public float lv_armor;
    public float lv_resist;
    public string effect_sign;
    public float[] attrLvRates;

    public override void ParseJson(object jd)
    {
        base.ParseJson(jd);

        id = long.Parse(item["monster_id"].ToString());

        types = int.Parse(item["types"].ToString());
        //describe = item["describe"].ToString();
        //info = item["info"].ToString();
        icon_name = item["icon_name"].ToString();
        model = int.Parse(item["model"].ToString());
        modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(model);
        released = int.Parse(item["released"].ToString());
        is_icon = int.Parse(item["is_icon"].ToString());
        if (null != item["skill_id"] && item["skill_id"] is int[])
        {
            int[] node = item["skill_id"] as int[];
            if (node != null)
            {
                skill_id = new long[node.Length];
                skills = new SkillNode[node.Length];
                for (int i = 0; i < node.Length; i++)
                {
                    skill_id[i] = long.Parse(node[i].ToString());
                    if (FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList.ContainsKey(skill_id[i]))
                    {
                        skills[i] = FSDataNodeTable<SkillNode>.GetSingleton().DataNodeList[skill_id[i]];
                        if (skills[i].site != 0) skillNodeDict.Add(skills[i].site, skills[i]);
                    }
                    else
                    {
                        GameDebug.LogError("策划好好看看怪物表");
                    }
                }
            }
        }
        if (item.ContainsKey("lv_hp"))
        {
            lv_hp = float.Parse(item["lv_hp"].ToString());
        }
        if (item.ContainsKey("lv_attack"))
        {
            lv_attack = float.Parse(item["lv_attack"].ToString());
        }
        if (item.ContainsKey("lv_magic"))
        {
            lv_magic = float.Parse(item["lv_magic"].ToString());
        }
        if (item.ContainsKey("lv_armor"))
        {
            lv_armor = float.Parse(item["lv_armor"].ToString());
        }
        if (item.ContainsKey("lv_resist"))
        {
            lv_resist = float.Parse(item["lv_resist"].ToString());
        }

        model_size = float.Parse(item["model_size"].ToString());
        effect_sign = item["effect_sign"].ToString();

    }
}
