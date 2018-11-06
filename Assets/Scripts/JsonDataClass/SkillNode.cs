using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
//伤害类型
public enum DamageType : byte
{
    physics = 1,//物理
    magic = 2,//魔法
    fix = 3//真实伤害
}
//技能类型 0：近战攻击；1：投掷；2：飞行；3：闪现；4：冲锋最大距离；5：冲击波；6：追踪目标；7：中心式；8：陷阱触发；9：召唤类；10：区域型；11：链式；12：移动中心式；13：链接式；14：爆炸；15:弹射；16：多目标追踪;
//17:移动扩散收缩；18：掘地；19：跳砍；20：多目标弹射；21:生成障碍物；22：扩散型；23：牵引；24：发射扩散米字型；25：游走；26：冲锋指定位置
public enum SkillCastType
{
    MeleeEffect = 0,
    CastSkill = 1,
    FlyEffect = 2,
    BlinkSkill = 3,
    FrontSprintSkill = 4,
    ShockWaveSkill = 5,
    TrackSkill = 6,
    CenterSkill = 7,
    TrapSkill = 8,
    SummonSkill = 9,
    AoeSKill = 10,
    LinkSkill = 11,
    ToSelfCenterSkill = 12,
    ChainSkill = 13,
    ThrowBoom = 14,
    Bounce = 15,
    MultiTrackSkill = 16,
    MoveShrinkSkill = 17,
    BurrowSkill = 18,
    JumpChopSkill = 19,
    MultiBounce = 20,
    GenerateObstacle = 21,
    DiffuseSkill = 22,
    MultiTractionSkill = 23,
    LaunchDiffuseMeterSkill = 24,
    SnakeSteerSkill = 25,
    FrontSprintSkill2 = 26,
    JumpBack = 27,
    TargetConnect = 28,
    MoveShoot = 29,
    ContinueAOE = 30,
    TractionForward = 31,
    BounceForward = 32,
    Recycle = 33,
    MultiZoneAoe = 34,
    AoeSKillMove = 35,
    MultiTrap = 36,
    Boom = 37,
}
//作用类型 0：自己：1：我方英雄；2：我方小兵；3：敌方英雄；4：敌方小兵；5：敌方防御塔；6：敌方基地；
public enum influence_type
{
    self = 0,
    selfHero = 1,
    selfMonster = 2,
    enemyHero = 3,
    enemyMonster = 4,
    enemyTower = 5,
    enemyBase = 6
}
//技能作用范围类型 0：无；1：溅射；2：被格挡；3：推进
public enum rangeType
{
    none = 0,
    spurting = 1,
    canBlock = 2,
    boost = 3,
}
//选目标规则 0：当前 1：随机 2：最远
public enum ChoseTarget
{
    none = 0,
    random = 1,
    farthest = 2,
}

//范围类型：第一位为类型，参数
//0：[子弹类, 圆半径]
//1：[矩形类型, 大圆半径，长，宽]
//2：[扇形类型, 大圆半径，角度]
//3：[圆形类型, 大圆半径，小圆半径]
//4:[全图矩形范围, 长，宽]
public enum RangenType
{
    OuterCircle = 0,
    OuterCircle_InnerCube = 1,
    OuterCircle_InnerSector = 2,
    OuterCircle_InnerCircle = 3,
    InnerCube = 4,
}

// 技能范围展示数据
public class RangenValue
{
    public RangenType type;
    public float length;
    public float width;
    public float outerRadius;
    public float innerRadius;
    public float angle;
}

public enum TargetState
{
    None = 0,
    Need = 1
}


public class SkillNode : FSDataNodeBase
{
    public long skill_id;//主技能ID
    public long hero_id;//所属英雄
    public string name;//所属英雄名称
    public string skill_name;//主技能名称
    public string des;//技能说明
    public string skill_atlas;//技能图标icon
    public string skill_icon;//技能图标icon

    public float cooling;//技能冷却时间/s
    public float dist;//施放距离
    public float castBefore;//施法前摇

    public string spell_motion;//施法动作
    public string spell_effect;//施法特效

    public string sound;//技能音效
    public string hit_sound;//技能命中音效
    public byte target_ceiling;//目标上限
    public byte attack_type;//攻击类型 1：普攻，2：普攻类技能，3：技能
    public byte types;//伤害类型  1：物理；2：法术；3：真实伤害；
    public bool isSingle;//0:单体；1：aoe;
    public bool isPierce;//是否为穿透技能
    public bool ignoreTerrain;//是否无视地形0:无视；1：不无视
    public float flight_speed;//飞行速度
    public float max_fly;//飞行最远距离
    public SkillCastType skill_type;//技能类型 0：近战攻击；1：投掷；2：飞行；3：闪现；4：冲锋；5：冲击破；6：追踪目标；7：中心式；8：陷阱触发；9：召唤类；10：区域型；11：链式；12：移动中心式
    public rangeType range_type;//技能作用范围类型 0：单体；1：分裂；2：溅射；3：中心；4：直线；5：圆形；6：矩形	
    public float aoe_long;//范围长度/半径
    public float aoe_wide;//范围宽度
    public float angle;//扇形角度
    public float length_base;//扇形底部长度
    public int site;//技能下标
    public int seat;//技能图标位置
    public int alertedType;//警告类型
    public int energy;//施放消耗值(豆)
    public int[] influence_type;//目标类型
    public int[] nullity_type;//目标无效类型 0：无；1：魔免；2：物免；3：建筑；
    public byte missable;//是否判断命中，1判断，0不判断
    public float efficiency_time;//技能作用时间//
    public float effect_time;//持续施法时间/s 0：瞬发
    public int isFiringPoint;//是否为发射点 0:脚底下；1：发射点
    public float[] interval_time;//间隔时间
    public float[] damage_ratio;//多段伤害伤害系数

    public int[] buffs_target;//buff作用类型
    public object[] specialBuffs;//特殊buff,buff持续时间和动作时间一致
    public object[] add_state;//附加状态[[状态1id,持续时间],[状态2id,持续时间]]
    public float[] base_num1;//基础数值[伤害,状态1,状态2]
    public float[] growth_ratio1;//成长系数[伤害,状态1,状态2]
    public float[] skill_ratio;//技能加成系数[伤害,状态1,状态2] 
    public int[] stats;//加成属性 [伤害,状态1,状态2] 0：无；1：power；2：intelligence；3：agility；4：hp；5：attack；6：armor；7：magic_resist；8：critical；9：dodge；10：hit_ratio；11：armor_penetration；12：magic_penetration；13：suck_blood；14：tenacity；15：movement_speed；16：attack_speed；17：striking_distance；

    public long[] skill_parts;

    public float range;
    public object[] battleEffects;
    public bool Cancelable = true;
    public TargetState target;
    public ChoseTarget choseTarget;
    public RangenValue rangenValue = new RangenValue();   //按下技能按键时，显示在英雄身边的作用范围数据

    public int castOrder = 0;
    public int casePriority = 0;
    public float animatorTime = 1;//动画时长

    public float warn_time = 1;
    public override void ParseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        skill_id = item.TryGetLong("skill_id");
        name = item.TryGetString("name");
        hero_id = item.TryGetLong("hero_id");
        skill_name = item.TryGetString("skill_name");
        des = item.TryGetString("des");
        skill_atlas = item.TryGetString("icon_atlas");
        skill_icon = item.TryGetString("skill_icon");
        spell_motion = item.TryGetString("skill_motion");
        sound = item.TryGetString("sound");
        hit_sound = item.TryGetString("hit_sound");
        target_ceiling = item.TryGetByte("target_ceiling");
        attack_type = item.TryGetByte("attack_type");
        types = item.TryGetByte("types");
        dist = item.TryGetFloat("dist");
        castBefore = item.TryGetFloat("end_motion");
        if (item.ContainsKey("singular_aoe"))
        {
            isSingle = item["singular_aoe"] == null ? true : byte.Parse(item["singular_aoe"].ToString()) == 0;
        }
        if (item.ContainsKey("ignore_terrain"))
        {
            ignoreTerrain = item["ignore_terrain"] == null ? false : byte.Parse(item["ignore_terrain"].ToString()) == 0;
        }
        if (item.ContainsKey("pierce through"))
        {
            isPierce = item["pierce through"] == null ? false : byte.Parse(item["pierce through"].ToString()) == 0;
        }
        flight_speed = item.TryGetFloat("flight_speed");
        if (item.ContainsKey("max_fly"))
        {
            max_fly = item["max_fly"] == null ? 0 : float.Parse(item["max_fly"].ToString());
        }
        skill_type = item.ContainsKey("skill_type") && item["skill_type"] != null ? (SkillCastType)byte.Parse(item["skill_type"].ToString()) : (SkillCastType)0;
        if (item.ContainsKey("range_type"))
        {
            range_type = (rangeType)byte.Parse(item["range_type"].ToString());
        }
        aoe_long = item.TryGetFloat("aoe_long");
        aoe_wide = item.TryGetFloat("aoe_wide");
        angle = item.TryGetFloat("angle");
        site = item.TryGetInt("site");
        seat = item.TryGetInt("seat");
        alertedType = item.TryGetInt("alerted_type");
        length_base = item.TryGetFloat("length_base");
        energy = item.TryGetInt("energy");
        if (item.ContainsKey("target"))
        {
            target = (TargetState)(int.Parse(item["target"].ToString()));
        }
        if (item.ContainsKey("choose_target"))
        {
            choseTarget = (ChoseTarget)(int.Parse(item["choose_target"].ToString()));
        }
        if (item.ContainsKey("rangen_type"))
        {
            object[] rangens = item["rangen_type"] as object[];
            if (rangens != null)
            {
                rangenValue.type = rangens.Length > 0 ? (RangenType)(int.Parse(rangens[0].ToString())) : RangenType.OuterCircle;
                switch (rangenValue.type)
                {
                    case RangenType.OuterCircle:
                        rangenValue.outerRadius = rangens.Length > 1 ? float.Parse(rangens[1].ToString()) : 0;
                        break;
                    case RangenType.OuterCircle_InnerCube:
                        rangenValue.outerRadius = rangens.Length > 1 ? float.Parse(rangens[1].ToString()) : 0;
                        rangenValue.length = rangens.Length > 2 ? float.Parse(rangens[2].ToString()) : 0;
                        rangenValue.width = rangens.Length > 3 ? float.Parse(rangens[3].ToString()) : 0;
                        break;
                    case RangenType.OuterCircle_InnerSector:
                        rangenValue.outerRadius = rangens.Length > 1 ? float.Parse(rangens[1].ToString()) : 0;
                        rangenValue.angle = rangens.Length > 2 ? float.Parse(rangens[2].ToString()) : 0;
                        break;
                    case RangenType.OuterCircle_InnerCircle:
                        rangenValue.outerRadius = rangens.Length > 1 ? float.Parse(rangens[1].ToString()) : 0;
                        rangenValue.innerRadius = rangens.Length > 2 ? float.Parse(rangens[2].ToString()) : 0;
                        break;
                    case RangenType.InnerCube:
                        rangenValue.length = rangens.Length > 1 ? float.Parse(rangens[1].ToString()) : 0;
                        rangenValue.width = rangens.Length > 2 ? float.Parse(rangens[2].ToString()) : 0;
                        break;
                    default:
                        break;
                }
            }
        }
        if (item.ContainsKey("interval_time"))
        {
            interval_time = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["interval_time"]);
        }
        if (item.ContainsKey("damage_ratio"))
        {
            damage_ratio = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["damage_ratio"]);
        }
        if (item.ContainsKey("nullity_type"))
        {
            int[] nodelist = item["nullity_type"] as int[];
            if (nodelist != null)
            {
                nullity_type = new int[nodelist.Length];

                for (int m = 0; m < nodelist.Length; m++)
                {
                    nullity_type[m] = nodelist[m];
                }
            }
        }
        if (item.ContainsKey("influence_type"))
        {
            int[] influenceList = item["influence_type"] as int[];
            if (influenceList != null)
            {
                influence_type = influenceList;
            }
        }
        missable = item.TryGetByte("missable");
        efficiency_time = item.TryGetFloat("efficiency_time");
        effect_time = item.TryGetFloat("effect_time");
        if (item.ContainsKey("firing"))
        {
            isFiringPoint = item.TryGetInt("firing");
        }
        cooling = item.TryGetFloat("cooling");
        if (item.ContainsKey("base_num1"))
        {
            base_num1 = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["base_num1"]);
        }
        if (item.ContainsKey("growth_ratio1"))
        {
            growth_ratio1 = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["growth_ratio1"]);
        }
        if (item.ContainsKey("skill_ratio"))
        {
            skill_ratio = FSDataNodeTable<SkillNode>.GetSingleton().ParseToFloatArray(item["skill_ratio"]);
            if (skill_ratio == null)
                Debug.LogError("skill_ratio null");
        }
        if (item.ContainsKey("stats"))
        {
            stats = item["stats"] as int[];
        }
        if (item.ContainsKey("buffs_target"))
        {
            buffs_target = item["buffs_target"] as int[];
        }
        if (item.ContainsKey("special_buffs"))
        {
            specialBuffs = (object[])item["special_buffs"];
        }
        if (item.ContainsKey("skill_parts"))
        {
            skill_parts = FSDataNodeTable<SkillNode>.GetSingleton().ParseToLongArray(item["skill_parts"]);
        }
        if (item.ContainsKey("add_state"))
        {
            add_state = item["add_state"] as object[];
        }
        range = item.TryGetFloat("dist");
        warn_time = item.TryGetFloat("warn_time");
        animatorTime = item.TryGetFloat("actuation time");
    }

    public bool IsSerialSkill()
    {
        return skill_parts.Length > 1;
    }

    public bool IsPartSkill()
    {
        return skill_parts.Length == 1;
    }

    private string c1 = "FF0000";
    private string c2 = "7030A0";
    private string c3 = "d18609";
    private string c4 = "6ae878";
    private string c5 = "FF9933";

    public string JointExtraDamageString(string extra, int index)
    {
        string temStr = "";
        if (float.Parse(extra) > 0)
        {
            if (index == 1 || index == 3)
            {
                temStr = "[" + c3 + "](+" + extra + ")[-]";
            }
            else if (index == 4)
            {
                temStr = "[" + c4 + "](+" + extra + ")[-]";
            }
            else
            {
                temStr = extra;
            }
        }
        else
        {
            //GameDebug.LogError("加成值为0");
        }
        return temStr;
    }

    static double[] GetEffectsConfig(object config)
    {
        if (config is int[])
        {
            int[] retObjArr = (int[])config;
            return Array.ConvertAll<int, double>(retObjArr, o => (int)o);
        }
        else if (config is object[])
        {
            object[] retObjArr = (object[])config;
            return Array.ConvertAll<object, double>(retObjArr, o => double.Parse(o.ToString()));
        }
        return null;
    }

    public bool isNormalAttack()
    {
        return spell_motion.StartsWith("attack");
    }
}
