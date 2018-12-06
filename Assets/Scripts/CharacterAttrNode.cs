using System.Collections.Generic;
using UnityEngine;

public class CharacterAttrNode : FSDataNodeBase
{
    protected Dictionary<string, object> item;

    public long id;

    public string name;// 名字

    public int types; // 5,boss
    //public string describe;
    public string info;
    public string icon_name;
    public int model;
    public ModelNode modelNode;
    public int released;
    public int is_icon;
    public long[] skill_id;
    public object[] skill1_id;
    public SkillNode[] skills;
    public Dictionary<int, SkillNode> skillNodeDict = new Dictionary<int, SkillNode>();
    public int hpBar_count = 6;


    #region 属性
    public int hp;//初始生命值
    public int mp;//初始法力值
    public float attack;//物理攻击
    public float magic;//初始法术攻击
    public float armor;//物理防御
    public float magic_resist;//法术防御
    public float speed_addition;//攻速加成
    public float critical_rate;//物理暴击率
    public float critical_effect;//物理暴击伤害
    public float magic_critical_rate;//法术暴击率
    public float magic_critical_effect;//法术暴击伤害
    public float movement_speed;//固定移速
    public float movement_add;//移动速度加成
    public float physical_sucking_rate;//物理吸血率
    public float magic_sucking_rate;//法术吸血率
    public float reduce_cd;//减CD
    public float armor_penetration;//初始护甲穿透
    public float armor_penetration_rate;//护甲穿透率
    public float magic_penetration;//初始魔法穿透
    public float magic_penetration_rate;//法术穿透率
    public float hp_regain;//生命恢复
    public float mp_regain;//回蓝
    public float regain_effect;//治疗效果
    public float tenacity_rate;//韧性
    public float physical_protection;//物理防护
    public float magic_protection;//法术防护
    public float physical_rebound;//物理反弹
    public float magic_rebound;//法术反弹
    public float dodge_rate;//闪避率
    public float hit_ratio;//命中率
    public float attack_hurt;//普攻伤害
    public float skill_hurt;//技能伤害
    public float striking_distance;//射程
    public float field_distance;//视野
    public float attack_speed;//固定攻速
    #endregion
    public float chase_range = 4f;//追击距离


    public float model_size = 1;

    public float[] base_Propers;//基础属性

    public float hurt_addition;//伤害加成
    public float hurt_remission;//伤害减免



    public float[] addition_Propers;//加成属性

    public float hp_addition;//生命值加成
    public float mp_addition;//法力值加成
    public float attack_addition;//物理攻击加成
    public float magic_addition;//法术攻击加成
    public float armor_addition;//物理防御加成
    public float magic_resist_addition;//法术防御加成

    public override void ParseJson(object jd)
    {
        item = jd as Dictionary<string, object>;

        if (item.ContainsKey("movement_speed"))
        {
            movement_speed = float.Parse(item["movement_speed"].ToString());
        }
        if (item.ContainsKey("striking_distance"))
        {
            striking_distance = float.Parse(item["striking_distance"].ToString());
        }
        if (item.ContainsKey("attack_hurt"))
        {
            attack_hurt = float.Parse(item["attack_hurt"].ToString());
        }
        if (item.ContainsKey("skill_hurt"))
        {
            skill_hurt = float.Parse(item["skill_hurt"].ToString());
        }

        if (item.ContainsKey("field_distance"))
        {
            field_distance = float.Parse(item["field_distance"].ToString());
        }
        if (item.ContainsKey("name"))
        {
            name = item["name"].ToString();
        }
        if (item.ContainsKey("hp"))
        {
            hp = Mathf.FloorToInt(System.Convert.ToInt32(item["hp"]));
        }
        if (item.ContainsKey("mp"))
        {
            mp = int.Parse(item["mp"].ToString());
        }
        if (item.ContainsKey("attack"))
        {
            attack = float.Parse(item["attack"].ToString());
        }
        if (item.ContainsKey("armor"))
        {
            armor = float.Parse(item["armor"].ToString());
        }
        if (item.ContainsKey("magic_resist"))
        {
            magic_resist = float.Parse(item["magic_resist"].ToString());
        }
        if (item.ContainsKey("hit_ratio"))
        {
            hit_ratio = float.Parse(item["hit_ratio"].ToString());
        }
        if (item.ContainsKey("physical_protection"))
        {
            physical_protection = float.Parse(item["physical_protection"].ToString());
        }
        if (item.ContainsKey("magic_protection"))
        {
            magic_protection = float.Parse(item["magic_protection"].ToString());
        }
        if (item.ContainsKey("physical_rebound"))
        {
            physical_rebound = float.Parse(item["physical_rebound"].ToString());
        }
        if (item.ContainsKey("magic_rebound"))
        {
            magic_rebound = float.Parse(item["magic_rebound"].ToString());
        }

        if (item.ContainsKey("armor_penetration"))
        {
            armor_penetration = float.Parse(item["armor_penetration"].ToString());
        }
        if (item.ContainsKey("magic_penetration"))
        {
            magic_penetration = float.Parse(item["magic_penetration"].ToString());
        }
        if (item.ContainsKey("tenacity_rate"))
        {
            tenacity_rate = float.Parse(item["tenacity_rate"].ToString());
        }
        if (item.ContainsKey("chase_distanc"))
            chase_range = float.Parse(item["chase_distanc"].ToString());
        if (item.ContainsKey("hpBar_count"))
            hpBar_count = int.Parse(item["hpBar_count"].ToString());

        if (item.ContainsKey("magic"))
        {
            magic = float.Parse(item["magic"].ToString());
        }
        if (item.ContainsKey("critical_rate"))
        {
            critical_rate = float.Parse(item["critical_rate"].ToString());
        }
        if (item.ContainsKey("magic_critical_rate"))
        {
            magic_critical_rate = float.Parse(item["magic_critical_rate"].ToString());
        }
        if (item.ContainsKey("magic_critical_effect"))
        {
            magic_critical_effect = float.Parse(item["magic_critical_effect"].ToString());
        }
        if (item.ContainsKey("critical_effect"))
        {
            critical_effect = float.Parse(item["critical_effect"].ToString());
        }
        if (item.ContainsKey("reduce_cd"))
        {
            reduce_cd = float.Parse(item["reduce_cd"].ToString());
        }
        if (item.ContainsKey("dodge_rate"))
        {
            dodge_rate = float.Parse(item["dodge_rate"].ToString());
        }
        if (item.ContainsKey("armor_penetration_rate"))
        {
            armor_penetration_rate = float.Parse(item["armor_penetration_rate"].ToString());
        }
        if (item.ContainsKey("magic_penetration_rate"))
        {
            magic_penetration_rate = float.Parse(item["magic_penetration_rate"].ToString());
        }
        if (item.ContainsKey("physical_sucking_rate"))
        {
            physical_sucking_rate = float.Parse(item["physical_sucking_rate"].ToString());
        }
        if (item.ContainsKey("magic_sucking_rate"))
        {
            magic_sucking_rate = float.Parse(item["magic_sucking_rate"].ToString());
        }
        if (item.ContainsKey("speed_addition"))
        {
            speed_addition = float.Parse(item["speed_addition"].ToString());
        }
        if (item.ContainsKey("attack_speed"))
        {
            attack_speed = float.Parse(item["attack_speed"].ToString());
        }
        if (item.ContainsKey("regain_effect"))
        {
            regain_effect = float.Parse(item["regain_effect"].ToString());
        }
        if (item.ContainsKey("mp_regain"))
        {
            mp_regain = float.Parse(item["mp_regain"].ToString());
        }
        if (item.ContainsKey("movement_add"))
        {
            movement_add = float.Parse(item["movement_add"].ToString());
        }
        if (item.ContainsKey("hp_regain"))
        {
            hp_regain = float.Parse(item["hp_regain"].ToString());
        }
        //    hurt_addition,//伤害加成
        //hurt_remission,//伤害减免
        //hp_addition,//生命值加成
        //mp_addition,//法力值加成
        //attack_addition,//物理攻击加成
        //magic_addition,//法术攻击加成
        //armor_addition,//物理防御加成
        //magic_resist_addition,//法术防御加成
        if (item.ContainsKey("hurt_addition"))
        {
            hurt_addition = float.Parse(item["hurt_addition"].ToString());
        }
        if (item.ContainsKey("hurt_remission"))
        {
            hurt_remission = float.Parse(item["hurt_remission"].ToString());
        }
        if (item.ContainsKey("hp_addition"))
        {
            hp_addition = float.Parse(item["hp_addition"].ToString());
        }
        if (item.ContainsKey("mp_addition"))
        {
            mp_addition = float.Parse(item["mp_addition"].ToString());
        }
        if (item.ContainsKey("attack_addition"))
        {
            attack_addition = float.Parse(item["attack_addition"].ToString());
        }
        if (item.ContainsKey("magic_addition"))
        {
            magic_addition = float.Parse(item["magic_addition"].ToString());
        }
        if (item.ContainsKey("armor_addition"))
        {
            armor_addition = float.Parse(item["armor_addition"].ToString());
        }
        if (item.ContainsKey("magic_resist_addition"))
        {
            magic_resist_addition = float.Parse(item["magic_resist_addition"].ToString());
        }
    }
}