using UnityEngine;

public class MonsterAttrNode : CharacterAttrNode
{
    public float lv_hp;
    public float lv_attack;
    public float lv_magic;
    public float lv_armor;
    public float lv_resist;


    //public float lv_critical;
    //public float lv_dodge;
    //public float lv_ratio;
    //public float lv_armorpenetration;
    //public float lv_magicpenetration;
    //public float lv_suckblood;
    //public float lv_tenacity;
    public string effect_sign;
    public int spawnAnimation;
    //public float correct;
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
        spawnAnimation = item.TryGetInt("spawnAnimation");
        if (spawnAnimation!=0)
        {
            Debug.Log("出场"+id);
        }
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

        // if (item.ContainsKey("correct"))
        // {
        //     correct = float.Parse(item["correct"].ToString());
        // }

        //lv_critical = float.Parse(item["lv_critical"].ToString());
        //lv_dodge = float.Parse(item["lv_dodge"].ToString());
        //lv_ratio = float.Parse(item["lv_ratio"].ToString());
        //lv_armorpenetration = float.Parse(item["lv_armorpenetration"].ToString());
        //lv_magicpenetration = float.Parse(item["lv_magicpenetration"].ToString());
        //lv_suckblood = float.Parse(item["lv_suckblood"].ToString());
        //lv_tenacity = float.Parse(item["lv_tenacity"].ToString());
        model_size = float.Parse(item["model_size"].ToString());
        effect_sign = item.TryGetString("effect_sign");

    }
}
