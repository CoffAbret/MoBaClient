using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
//伤害类型
public enum DamageType : byte
{
    physics = 1,//物理
    magic = 2,//魔法
    fix = 3,//真实伤害
    cure = 4,//治疗
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
    Around = 38,
}
//作用类型 1：自己：2：我方小兵；3：敌方小兵4：我方英雄；5：敌方英雄；
public enum influence_type
{
    self = 1,
    selfMonster = 2,
    enemyMonster = 3,
    selfHero = 4,
    enemyHero = 5,
}

public enum col_type
{
    self = 0,
    selfMonster = 1,
    selfHero = 2,
    enemyMonster = 3,
    enemyHero = 4,
    selfTower = 5,
    enemyTower = 6,
    neutralMonster = 7,
    neutralTower = 8,
    terrain = 9,
    target = 10,
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

//动作中朝向
//"1.释放时朝向不变
//2.跟随目标方向转动
//3.可手动调整方向
//4.等同位移方向
public enum FaceType
{
    None = 0,
    Hold = 1,
    FollowTarget = 2,
    Handle = 3,
    MoveForward = 4,
}
//释放类型
//"1.方向型
//2.坐标点型
//3.直接释放型
//4.目标型
public enum SkillUseType
{
    None = 0,
    Direction = 1,
    Point = 2,
    Forward = 3,
    Target = 4,
}
//子弹目标类型
//"0.自身
//1.当前目标
//2.范围内目标"
public enum BulTargetType
{
    Self = 0,
    Target = 1,
    Range = 2,
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

    public int alerted_position;//boss预警位置：0特效位置 1目标位置

    public float range;
    public object[] battleEffects;
    public bool Cancelable = true;
    public ChoseTarget choseTarget;
    public RangenValue rangenValue = new RangenValue();   //按下技能按键时，显示在英雄身边的作用范围数据

    public int castOrder = 0;
    public int casePriority = 0;
    public float animatorTime = 1;//动画时长

    public float warn_time = 1;


    public object[] life_drain;//技能吸血参数 [a,b]a:1固定值2百分比b:参数（大于0）
    //--------------------------------------------------------------------------------------
    //新加数据
    public int energyvalue;//施放消耗值
    public bool can_move;//动作中位移
    public bool is_turnround;//释放前转向
    public FaceType face_type;//动作中朝向
    public string[] effect;//动作特效
    public int[] effect_position;//特效挂点
    public List<Vector3> effect_positionxyz = new List<Vector3>();//特效挂点位置偏移
    public int[] effect_start;//施法特效生成时间
    public double[] effect_end;//施法特效存在时间
    public SkillUseType skill_usetype;//释放类型
    public double[] bullet_time;//子弹触发时间点
    public int[] bullet_id;//子弹id
    public int[] bul_target_type;//子弹目标类型
    public List<int[]> bul_target_value;//子弹目标类型参数
    public int[] bul_target_size;//子弹目标范围参数
    public int[] max_bul;//子弹最大数量
    public List<int[]> bul_num_single;//同目标最小，最大数量
    public int[] bul_start;//子弹发射源" 1.自身 2.当前目标"
    public List<Vector3> firing_xyz = new List<Vector3>();//子弹发射挂点偏移
    public int bul_end;//子弹目标挂点 0:脚底下；1：发射点
    public List<Vector3> bul_end_xyz = new List<Vector3>();//子弹目标挂点偏移
    public int[] bul_end_angle;//子弹目标挂点偏移角度
    public int[] bul_son_max;//子子弹触发最大轮数
    public int[] combo_time;//普攻连击限制
    //--------------------------------------------------------------------------------------

    public override void ParseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        skill_id = item.TryGetLong("skillid");
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
                GameDebug.LogError("skill_ratio null");
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
        // nullity_type = byte.Parse(item["nullity_type"].ToString());
        animatorTime = item.TryGetFloat("actuation time");
        if (item.ContainsKey("life_drain"))
        {
            life_drain = item["life_drain"] as object[];
        }
        if (item.ContainsKey("alerted_position"))
        {
            alerted_position = item.TryGetInt("alerted_position");
        }

        //--------------------------------------------------------------------------------------
        //新加数据
        energyvalue = item.TryGetInt("energy_value");
        can_move = item.TryGetInt("can_move") == 2;
        is_turnround = item.TryGetInt("is_turnround") == 1;
        face_type = (FaceType)item.TryGetInt("face_type");
        effect = item.TryGetStringIntArr("effect");
        effect_position = item.TryGetIntArr("effect_position");
        if (item.ContainsKey("effect_positionxyz") && item["effect_positionxyz"] != null)
        {
            object[] effect_positionxyz_temp = item["effect_positionxyz"] as object[];
            GetVector3List(effect_positionxyz, effect_positionxyz_temp);
        }
        effect_start = item.TryGetIntArr("effect_start");
        effect_end = item.TryGetDoubleArr("effect_end");
        skill_usetype = (SkillUseType)item.TryGetInt("skill_usetype");
        bullet_time = item.TryGetDoubleArr("bullet_time");
        bullet_id = item.TryGetIntArr("bullet_id");
        bul_target_type = item.TryGetIntArr("bul_target_type");
        if (item.ContainsKey("bul_target_value") && item["bul_target_value"] != null)
        {
            bul_target_value = new List<int[]>();
            object[] bul_target_value_temp = item["bul_target_value"] as object[];
            for (int i = 0; i < bul_target_value_temp.Length; i++)
            {
                int[] objs = bul_target_value_temp[i] as int[];
                bul_target_value.Add(objs);
            }
        }
        bul_target_size = item.TryGetIntArr("bul_target_size");
        max_bul = item.TryGetIntArr("max_bul");
        if (item.ContainsKey("bul_num_single") && item["bul_num_single"] != null)
        {
            bul_num_single = new List<int[]>();
            object[] bul_num_single_temp = item["bul_num_single"] as object[];
            for (int i = 0; i < bul_num_single_temp.Length; i++)
            {
                int[] objs = bul_num_single_temp[i] as int[];
                bul_num_single.Add(objs);
            }
        }
        bul_start = item.TryGetIntArr("bul_start");
        isFiringPoint = item.TryGetInt("firing");
        if (item.ContainsKey("firing_xyz") && item["firing_xyz"] != null)
        {
            object[] firing_xyz_temp = item["firing_xyz"] as object[];
            GetVector3List(firing_xyz, firing_xyz_temp);
        }
        bul_end = item.TryGetInt("bul_end");
        if (item.ContainsKey("bul_end_xyz") && item["bul_end_xyz"] != null)
        {
            object[] bul_end_xyz_temp = item["bul_end_xyz"] as object[];
            GetVector3List(bul_end_xyz, bul_end_xyz_temp);
        }

        bul_end_angle = item.TryGetIntArr("bul_end_angle");
        bul_son_max = item.TryGetIntArr("bul_son_max");
        combo_time = item.TryGetIntArr("combo_time");

        //--------------------------------------------------------------------------------------

    }

    public void GetVector3List(List<Vector3> list_temp, object[] object_temp)
    {
        if (object_temp == null || object_temp.Length == 0)
        {
            //Debug.LogError("     SkillNode    ");
            return;
        }
        for (int i = 0; i < object_temp.Length; i++)
        {
            double[] objs = object_temp[i] as double[];
            if (objs != null && objs.Length == 3)
            {
                Vector3 temp = new Vector3(objs[0].ToString().StringToFloat(), objs[1].ToString().StringToFloat(), objs[2].ToString().StringToFloat());
                list_temp.Add(temp);
            }
        }
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

    long GetBuffId(object buffConfig)
    {
        if (buffConfig is Array && ((Array)buffConfig).Length > 0)
        {
            return Convert.ToInt64(((Array)buffConfig).GetValue(0));
        }
        return 0;
    }
}
