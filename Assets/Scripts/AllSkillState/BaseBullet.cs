using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ValueClass
{
    public Fix64 m_BulletId;//子弹id
    public Fix64 m_BulletIndex;//子弹记录序号
    public Fix64 m_bul_target_type;//子弹目标类型
    public Fix64 m_bul_target_value;//子弹目标类型参数
    public Fix64 m_bul_target_size;//子弹目标范围参数
    public Fix64 m_bul_start;//子弹发射源" 1.自身 2.当前目标"
    public FixVector3 m_firing_xyz = FixVector3.Zero;//子弹发射挂点偏移
    public Fix64 m_bul_end;//子弹目标挂点 0:脚底下；1：发射点
    public FixVector3 m_bul_end_xyz = FixVector3.Zero;//子弹目标挂点偏移
    public Fix64 m_bul_end_angle;//子弹目标挂点偏移角度
    public Fix64 m_bul_son_max;//子子弹触发最大轮数
    public Fix64 m_max_bul;//子弹最大数量

    //虚拟目标 1,2,3 通用
    public FixVector3 v_pos;
    //虚拟目标
    public List<Player> v_taregt = new List<Player>();
    //记录上次伤害的地址
    public FixVector3 old_pos;
    //虚拟目标
    public Player m_taregt;

    //创建子子弹参数
    public Fix64 newbul_origin;//触发子弹发射者
    public Fix64 newbul_target_extra;//目标额外判定
    public Fix64[] newbul_num_single;//同目标最小，最大数量
    public Fix64 son_now = Fix64.Zero;//子子弹触发当前轮数
    public Fix64 pen_now = Fix64.Zero;//子弹穿透次数
}
//碰撞记录
public class BeHurtTarget
{
    public Fix64 id = Fix64.Zero;//id
    public Player PlayerTarget;//玩家目标
    public Tower TowerTarget;//塔目标
    public Fix64 ThroughCount = Fix64.Zero;//穿透次数
    public Fix64 CollisionCount = Fix64.Zero;//碰撞次数
    public Fix64 SameTargetHurtCount;//同一目标伤害次数
}

public class BaseBullet : BaseState
{
    //技能node
    public SkillNode m_SkillNode;
    //子弹node
    public BulletNode m_BulletNode;
    //子弹传入参数类
    public Bullet_ValueClass m_BulletClass;
    //位置
    public FixVector3 m_Pos = FixVector3.Zero;
    //朝向
    public FixVector3 m_Angles = FixVector3.Zero;
    //技能是否有效
    public bool m_IsActive = false;
    //受伤角色列表
    public List<Player> m_WoundPlayerList;
    //受伤箭塔列表
    public List<Tower> m_WoundTowerList;
    //受伤目标列表
    public List<BeHurtTarget> m_BuHurtList;
    //碰撞总次数
    public Fix64 m_AllColCount = Fix64.Zero;
    //累计时间
    public Fix64 m_AttackTime;
    //选择位置
    public FixVector3 m_TargetPos = FixVector3.Zero;
    //伤害触发时间
    public Fix64 m_HurtTime = Fix64.FromRaw(0);
    //伤害触发结束时间
    public Fix64 m_HurtEndTime = Fix64.FromRaw(0);
    //间断性伤害判断
    public bool m_Isinterval = false;
    //当前伤害次数
    public Fix64 m_HurtCount = (Fix64)0;
    //计算目标位置与当前位置的间隔距离
    public FixVector3 m_FramePos;
    //检测前目标
    public Player m_Target;
    //目标玩家list
    public List<Player> m_TargetList = new List<Player>();
    //命中瞬移
    private bool Blink = false;

    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //技能显示范围
    public GameObject obj;
    //特效
    private GameObject m_AniEffect;
    //受击特效
    private GameObject m_AniHitEffect;
#endif
    #endregion

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public void CreateBullet(Player player, Bullet_ValueClass bullet, string parameter = null)
    {
        if (player == null || bullet == null)
            return;
        m_Player = player;
        m_Parameter = parameter;
        m_SkillNode = player.m_SkillNode;
        m_BulletClass = bullet;
        m_BulletNode = FSDataNodeTable<BulletNode>.GetSingleton().FindDataByType((long)bullet.m_BulletId);
        if (m_BulletNode == null)
        {
            Debug.Log(player.m_VGo.name + "     错误的子弹id      " + bullet.m_BulletId);
            return;
        }
        #region 子弹目标类型
        if (m_BulletClass.m_bul_target_type == (Fix64)0)//自身
        {
            m_Target = m_Player;
        }
        else if (m_BulletClass.m_bul_target_type == (Fix64)1)//当前目标
        {
            m_Target = m_BulletClass.m_taregt;
        }
        else if (m_BulletClass.m_bul_target_type == (Fix64)2)//范围内目标
        {
            m_Target = CheckBulTargetValue();
            //if (m_BulletClass.v_taregt.Count > 0)
            //{
            //    m_Target = m_BulletClass.v_taregt[0];
            //}
        }
        #endregion

        m_IsActive = true;

        #region 技能判断子弹发射者
        if (m_BulletClass.m_bul_start == (Fix64)1)//自身
        {
            m_Pos = player.m_Pos;
        }
        else if (m_BulletClass.m_bul_start == (Fix64)2)//当前目标
        {
            if (m_Target == null)
                return;
            m_Pos = m_Target.m_Pos;
        }
        #endregion

        #region 子子弹判断子弹发射者
        if (m_BulletClass.newbul_origin == (Fix64)0)//无
        {
        }
        else if (m_BulletClass.newbul_origin == (Fix64)1)//当前子弹碰撞目标
        {
            if (m_BulletClass.old_pos != FixVector3.Zero)
            {
                m_Pos = m_BulletClass.old_pos;
            }
        }
        else if (m_BulletClass.newbul_origin == (Fix64)2)//技能释放者
        {
            m_Pos = player.m_Pos;
        }
        else if (m_BulletClass.newbul_origin == (Fix64)3)//当前子弹
        {
            if (m_BulletClass.m_taregt != null)
            {
                m_Pos = m_BulletClass.m_taregt.m_Pos;
            }
            if (m_BulletClass.old_pos != FixVector3.Zero)
            {
                m_Pos = m_BulletClass.old_pos;
            }
        }
        #endregion

        m_BuHurtList = new List<BeHurtTarget>();

        //子弹结束位置
        if (m_Target != null)
        {
            m_BulletClass.v_pos = (FixVector3)m_Target.m_Pos + bullet.m_bul_end_xyz;
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            #region  加载子弹特效
            if (!string.IsNullOrEmpty(m_BulletNode.effect))
            {
                string eff = m_BulletNode.effect;
                if (m_Player.m_PlayerData.m_Type == 1 && !string.IsNullOrEmpty(eff))
                {
                    GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", m_Player.m_PlayerData.m_HeroName, eff));
                    if (effectGo != null)
                        m_AniEffect = GameObject.Instantiate(effectGo);
                }
                if (m_Player.m_PlayerData.m_Type == 2 && !string.IsNullOrEmpty(eff))
                {
                    GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Monster", m_Player.m_PlayerData.m_HeroName, eff));
                    if (effectGo != null)
                        m_AniEffect = GameObject.Instantiate(effectGo);
                }
                //if (m_AniEffect == null)
                //{
                //    m_AniEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    m_AniEffect.transform.localScale = new Vector3((float)1, (float)1, (float)1f);
                //}
                if (m_AniEffect == null)
                    return;
                //技能判断子弹发射者
                if (m_BulletClass.m_bul_start == (Fix64)1)//自身
                {
                    #region 后修改为挂点
                    //m_AniEffect.transform.parent = m_Player.m_VGo.transform;
                    m_AniEffect.transform.position = m_Player.m_Pos.ToVector3();
                    #endregion
                }
                else if (m_BulletClass.m_bul_start == (Fix64)2)//当前目标
                {
                    if (m_Target == null)
                        return;
                    #region 后修改为挂点
                    //m_AniEffect.transform.parent = m_Target.m_VGo.transform;
                    m_AniEffect.transform.position = m_Target.m_Pos.ToVector3();
                    #endregion
                }
                //子子弹判断子弹发射者
                if (m_BulletClass.newbul_origin == (Fix64)0)//无
                {

                }
                else if (m_BulletClass.newbul_origin == (Fix64)1)//当前子弹碰撞目标
                {
                    #region 后修改为挂点
                    if (m_BulletClass.old_pos != FixVector3.Zero)
                    {
                        m_AniEffect.transform.position = m_BulletClass.old_pos.ToVector3();
                    }
                    #endregion
                }
                else if (m_BulletClass.newbul_origin == (Fix64)2)//技能释放者
                {
                    #region 后修改为挂点
                    //m_AniEffect.transform.parent = m_Player.m_VGo.transform;
                    m_AniEffect.transform.position = m_Player.m_Pos.ToVector3();
                    #endregion
                }
                else if (m_BulletClass.newbul_origin == (Fix64)3)//当前子弹
                {
                    #region 后修改为挂点
                    if (m_BulletClass.m_taregt != null)
                    {
                        //m_AniEffect.transform.parent = m_Player.m_VGo.transform;
                        m_AniEffect.transform.position = m_BulletClass.m_taregt.m_Pos.ToVector3();
                    }
                    if (m_BulletClass.old_pos != FixVector3.Zero)
                    {
                        m_AniEffect.transform.position = m_BulletClass.old_pos.ToVector3();
                    }
                    #endregion
                }

                //m_AniEffect.transform.localPosition = Vector3.zero + m_BulletNode.effect_xyz;
                m_AniEffect.transform.localRotation = Quaternion.Euler(m_Player.m_Rotation.ToVector3());
                //m_AniEffect.transform.localScale = Vector3.one;
                if (m_BulletNode.effect_timeend != 0)//自然销毁特效
                {
                    Delay end_delay = new Delay();
                    end_delay.InitDestory(m_AniEffect, (Fix64)m_BulletNode.effect_timeend);
                    GameData.m_GameManager.m_DelayManager.m_DelayList.Add(end_delay);
                }
                else if ((Fix64)m_BulletNode.time_max > (Fix64)0)
                {
                    Delay end_delay = new Delay();
                    end_delay.InitDestory(m_AniEffect, (Fix64)m_BulletNode.time_max);
                    GameData.m_GameManager.m_DelayManager.m_DelayList.Add(end_delay);
                }
            }
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (m_Player == null)
            return;
        if (m_BulletNode == null)
            return;
        if (m_BulletNode.time_max > 0 && m_AttackTime >= (Fix64)m_BulletNode.time_max)
        {
            m_IsActive = false;
            OnExit();
            return;
        }
        m_AttackTime += GameData.m_FixFrameLen;

        if (m_BulletNode.fly_max == -1 || (m_BulletNode.fly_max > 0 && (Fix64)m_BulletNode.fly_max >= (Fix64)m_BulletNode.fly_speed * m_AttackTime))
        {
            Vector2 gridPos = GameData.m_GameManager.m_GridManager.MapPosToGrid((m_Pos + m_Angles * (Fix64)m_BulletNode.fly_speed * GameData.m_FixFrameLen).ToVector3());
            bool isWalk = GameData.m_GameManager.m_GridManager.GetWalkable(gridPos);
            //Debug.Log(gridPos + "               " + isWalk);
            if (isWalk)
            {
                Target_TypeMove();
                Follow_TypeMove();
            }
            //else {
            //    Vector2 gridPos11 = GameData.m_GameManager.m_GridManager.MapPosToGrid((m_Pos - m_Angles * (Fix64)m_BulletNode.fly_speed * GameData.m_FixFrameLen).ToVector3());
            //    bool isWalk11= GameData.m_GameManager.m_GridManager.GetWalkable(gridPos11);
            //}
        }
        Col_Times_Space();
        //if (m_BulletClass.m_bul_target_type != (Fix64)1)
        //{
        //    GetRangeTarget((FixVector3)m_Target.m_VGo.transform.position, bullet);
        //}
    }

    public void Target_TypeMove()//目的地类型
    {
        if (m_BulletNode == null)
            return;
        //Debug.Log("跟随目标移动");
        if (m_IsActive)
        {
            if (m_BulletNode.target_type == 1)//目标坐标
            {
                FixVector3 temp = m_BulletClass.v_pos - m_Pos;
                Vector3 v3_temp = new Vector3((float)temp.x, (float)temp.y, (float)temp.z);
                m_Pos = m_Pos + (FixVector3)v3_temp.normalized * ((Fix64)m_BulletNode.fly_speed * GameData.m_FixFrameLen);
                #region 显示层
                if (GameData.m_IsExecuteViewLogic)
                {
                    if (m_AniEffect != null)
                    {
                        m_AniEffect.transform.position = m_Pos.ToVector3();
                    }
                }
                #endregion
            }
            else if (m_BulletNode.target_type == 2)//追踪目标
            {
                if (m_Target == null)
                    return;
                FixVector3 temp = (FixVector3)m_Target.m_Pos - m_Pos;
                Vector3 v3_temp = new Vector3((float)temp.x, (float)temp.y, (float)temp.z);
                m_Pos = m_Pos + (FixVector3)v3_temp.normalized * ((Fix64)m_BulletNode.fly_speed * GameData.m_FixFrameLen);
                #region 显示层
                if (GameData.m_IsExecuteViewLogic)
                {
                    if (m_AniEffect != null)
                    {
                        m_AniEffect.transform.LookAt(m_Pos.ToVector3());
                        m_AniEffect.transform.position = m_Pos.ToVector3();
                    }
                }
                #endregion
            }
        }
    }

    public void Follow_TypeMove()//角色跟随处理
    {
        if (m_BulletNode == null)
            return;
        //Debug.Log("角色跟随处理");
        if (m_IsActive)
        {
            //if (m_BulletNode.fly_max == -1 || (m_BulletNode.fly_max > 0 && (Fix64)m_BulletNode.fly_speed * m_AttackTime < (Fix64)m_BulletNode.fly_max && m_BulletNode.time_max > 0)
            //{
            if (m_BulletNode.follow_type == 0)//无跟随处理
            {

            }
            else if (m_BulletNode.follow_type == 1)//子弹发射者跟随该子弹
            {
                m_Player.m_Pos = m_Pos;
                //m_Player.m_Angles = m_Angles;
                #region 显示层
                if (GameData.m_IsExecuteViewLogic)
                {
                    m_Player.m_VGo.transform.position = m_Player.m_Pos.ToVector3();
                    //m_Player.m_VGo.transform.rotation = Quaternion.Euler(m_Player.m_Angles.ToVector3());
                }
                #endregion
            }
            else if (m_BulletNode.follow_type == 2)//子弹命中时子弹发射者瞬移到命中坐标
            {
                Blink = true;
            }
            else if (m_BulletNode.follow_type == 3)//子弹命中时技能使用者瞬移到命中坐标
            {

            }
            //}
        }
    }

    public void Col_Times_Space()//物理检测 
    {
        if (m_BulletNode == null)
            return;
        if (m_BulletNode.col_times_max != -1 && m_AllColCount >= (Fix64)m_BulletNode.col_times_max)//碰撞限制最大数量
            return;
        //Debug.Log("物理检测");
        if (m_IsActive)
        {
            if (m_BulletNode.col_times_space.Length > 0)
            {
                if (m_BulletNode.col_times_space.Length == 1 && m_BulletNode.col_times_space[0] == 0)//无检测间隔
                {
                    HitDamage(GetRangeTarget(m_Pos));
                    //m_IsActive = false;
                    //OnExit();
                }
                else if (m_BulletNode.col_times_space.Length == 1 && m_BulletNode.col_times_space[0] != 0)//检测间隔为固定值
                {
                    if (m_AttackTime * (Fix64)50 >= (Fix64)m_BulletNode.col_times_space[0] * m_HurtCount)
                    {
                        m_HurtCount = m_HurtCount + (Fix64)1;
                        HitDamage(GetRangeTarget(m_Pos));
                    }
                }
                else if (m_BulletNode.col_times_space.Length > 1)//检测间隔为一整个数组
                {
                    if (m_HurtCount < (Fix64)m_BulletNode.col_times_space.Length && m_AttackTime * (Fix64)50 >= (Fix64)m_BulletNode.col_times_space[(int)m_HurtCount])
                    {
                        m_HurtCount = m_HurtCount + (Fix64)1;
                        HitDamage(GetRangeTarget(m_Pos));
                        //GetPlayerRangeList(m_Pos);
                        //GetTowerRangeList(m_Pos);
                    }
                }
            }
            else
            {
                HitDamage(GetRangeTarget(m_Pos));
                m_IsActive = false;
                OnExit();
            }
        }
    }


    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_Player == null)
            return;
        CreateSonBullet(1);
        m_BuHurtList.Clear();
        //m_BuHurtList = null;
        //m_WoundPlayerList.Clear();
        //m_WoundPlayerList = null;
        //m_WoundTowerList.Clear();
        //m_WoundTowerList = null;
    }

    public List<Player> GetRangeTarget(FixVector3 m_Pos)
    {
        List<Player> player = new List<Player>();
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i] == null || GameData.m_PlayerList[i].m_PlayerData == null)
                continue;
            if (m_BulletNode.col_type != null && m_BulletNode.col_type.Length > 0)
            {
                if (CheckHitCondition(m_BulletNode, m_Player, GameData.m_PlayerList[i]) && IsInRange(m_BulletNode, m_Player, GameData.m_PlayerList[i]))
                {
                    player.Add(GameData.m_PlayerList[i]);
                }
            }
        }
        //for (int i = 0; i < GameData.m_TowerList.Count; i++)
        //{
        //    if (GameData.m_TowerList[i] == null || GameData.m_TowerList[i].m_VGo == null)
        //        continue;
        //    if (GameData.m_TowerList[i].m_CampId == m_Player.m_PlayerData.m_CampId)
        //        continue;
        //    if (m_BulletClass.m_bul_target_type != (Fix64)2 && m_BulletClass.m_bul_target_value.Length > 0)
        //    {
        //        //子弹与敌人的方向向量
        //        FixVector3 targetV3 = GameData.m_TowerList[i].m_Pos - m_Pos;
        //        //求玩家正前方、玩家与敌人方向两个向量的夹角
        //        Fix64 angle = FixVector3.Angle(m_Angles, targetV3);
        //        Fix64 distance = FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos);
        //        if (((float)angle <= m_SkillNode.angle / 2 || m_SkillNode.angle <= 0) && (Fix64)distance <= m_BulletClass.m_bul_target_size && !m_WoundTowerList.Contains(GameData.m_PlayerList[i]))
        //        {
        //            player.Add(m_Target);
        //        }
        //    }
        //}
        return player;
    }

    public bool IsInRange(BulletNode m_BulletNode, Player attack, Player Target)
    {
        if (m_BulletNode.col_size_value == null)
            return false;
        float angle = 0;
        Fix64 distance = Fix64.Zero;
        switch (m_BulletNode.col_size_type)
        {
            case 1://扇形/环形
                if (m_BulletNode.col_size_value != null && m_BulletNode.col_size_value.Length == 4)
                {
                    angle = Mathf.Acos(Vector3.Dot(m_Player.m_Angles.ToVector3().normalized, (Target.m_Pos - attack.m_Pos).ToVector3().normalized)) * Mathf.Rad2Deg;
                    distance = Target.m_PlayerData.m_Type == 1 ? (FixVector3.Distance(Target.m_Pos, m_Pos) - Fix64.FromRaw(200)) : (FixVector3.Distance(Target.m_Pos, m_Pos) - Fix64.FromRaw(100));
                    if ((angle <= m_BulletNode.col_size_value[0] || m_BulletNode.col_size_value[0] <= 0) && ((float)distance <= m_BulletNode.col_size_value[1]) && ((float)distance >= m_BulletNode.col_size_value[2]))
                    {
                        return true;
                    }
                }
                break;
            case 2://矩形
                if (m_BulletNode.col_size_value != null && m_BulletNode.col_size_value.Length == 5)
                {
                    FixVector3 p1 = new FixVector3(m_Pos.x - (Fix64)m_BulletNode.col_size_value[2], m_Pos.y, m_Pos.x + (Fix64)m_BulletNode.col_size_value[0]);
                    FixVector3 p2 = new FixVector3(m_Pos.x + (Fix64)m_BulletNode.col_size_value[3], m_Pos.y, m_Pos.x + (Fix64)m_BulletNode.col_size_value[0]);
                    FixVector3 p3 = new FixVector3(m_Pos.x + (Fix64)m_BulletNode.col_size_value[3], m_Pos.y, m_Pos.x - (Fix64)m_BulletNode.col_size_value[1]);
                    FixVector3 p4 = new FixVector3(m_Pos.x - (Fix64)m_BulletNode.col_size_value[2], m_Pos.y, m_Pos.x - (Fix64)m_BulletNode.col_size_value[1]);
                    FixVector3 p = Target.m_Pos;
                    return GetCross(p1, p2, p) * GetCross(p3, p4, p) >= (Fix64)0 && GetCross(p2, p3, p) * GetCross(p4, p1, p) >= (Fix64)00;
                }
                break;
            default:
                break;
        }
        return false;
    }
    //检测矩形
    public Fix64 GetCross(FixVector3 p1, FixVector3 p2, FixVector3 p)
    {
        return (p2.x - p1.x) * (p.z - p1.z) - (p.x - p1.x) * (p2.z - p1.z);
    }
    //伤害计算
    public void HitDamage(List<Player> playerTargetList)
    {
        for (int i = 0; i < playerTargetList.Count; i++)
        {
            if (m_BulletNode.col_times_oneupdate != -1 && i >= m_BulletNode.col_times_oneupdate)//单次心跳碰撞限制最大数量
                return;
            m_AllColCount = m_AllColCount + (Fix64)1;//碰撞总次数
            float base_num1 = m_SkillNode.base_num1[0];
            float growth_ratio = m_SkillNode.growth_ratio1[0];
            float skill_ratio = m_SkillNode.stats[0];
            int stats = m_SkillNode.stats[0];
            float attack = m_Player.m_PlayerData.m_HeroAttrNode.attack;
            float armor = playerTargetList[i].m_PlayerData.m_HeroAttrNode.armor;
            float attack_hurt = m_Player.m_PlayerData.m_HeroAttrNode.attack_hurt;
            float hurt_addition = m_Player.m_PlayerData.m_HeroAttrNode.hurt_addition;
            float hurt_remission = playerTargetList[i].m_PlayerData.m_HeroAttrNode.hurt_remission;
            //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
            int damage = ((int)(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission) * (double)m_BulletNode.damage));//伤害
            damage = Mathf.Abs(damage);
            #region 检测物理碰撞最大次数
            if (m_BulletNode.col_times_single == -1)
            {
                m_BulletClass.m_taregt = playerTargetList[i];
                playerTargetList[i].FallDamage(damage);
                CreateSonBullet(2, playerTargetList[i].m_Pos.ToVector3());
            }
            else
            {
                bool haveTarget = false;
                for (int j = 0; j < m_BuHurtList.Count; j++)
                {
                    if (m_BuHurtList[j].PlayerTarget == playerTargetList[i])
                    {
                        haveTarget = true;
                        if ((Fix64)m_BulletNode.col_times_single >= m_BuHurtList[j].CollisionCount)
                        {
                            continue;
                        }
                        else
                        {
                            m_BulletClass.m_taregt = playerTargetList[i];
                            playerTargetList[i].FallDamage(damage);
                            m_BuHurtList[j].CollisionCount = m_BuHurtList[j].CollisionCount + (Fix64)1;
                            CreateSonBullet(2, playerTargetList[i].m_Pos.ToVector3());
                            m_BuHurtList[j].SameTargetHurtCount = m_BuHurtList[j].SameTargetHurtCount + (Fix64)1;
                        }
                    }
                }
                if (!haveTarget)
                {
                    m_BulletClass.m_taregt = playerTargetList[i];
                    playerTargetList[i].FallDamage(damage);
                    BeHurtTarget target = new BeHurtTarget();
                    target.PlayerTarget = playerTargetList[i];
                    m_BuHurtList.Add(target);
                    CreateSonBullet(2, playerTargetList[i].m_Pos.ToVector3());
                }
            }
            #endregion

            if (Blink)
            {
                m_Player.m_Pos = playerTargetList[i].m_Pos;
                #region 显示层
                if (GameData.m_IsExecuteViewLogic)
                {
                    m_Player.m_VGo.transform.position = m_Player.m_Pos.ToVector3();
                }
                #endregion
            }

            #region 穿透总次数限制
            int pen_time = ReturnPen_times_max(m_Player, playerTargetList[i]);
            if (pen_time != -10)
            {
                if (pen_time == -1)
                {

                }
                else if (pen_time == -2)
                {

                }
                else if (pen_time == -3)
                {
                    m_IsActive = false;
                    OnExit();
                }
                else if (pen_time >= 0 && (Fix64)m_BulletNode.pen_times_max[(int)m_BulletClass.m_BulletIndex] == m_BulletClass.pen_now)
                {
                    m_IsActive = false;
                    OnExit();
                }
            }
            m_BulletClass.pen_now = m_BulletClass.pen_now + (Fix64)1;
            #endregion
            #region 单目标穿透最大次数
            if (m_BulletNode.pen_times_singe == -1)
            {
            }
            else
            {
                bool haveTarget = false;
                for (int j = 0; j < m_BuHurtList.Count; j++)
                {
                    if (m_BuHurtList[j].PlayerTarget == playerTargetList[i])
                    {
                        haveTarget = true;
                        if ((Fix64)m_BulletNode.pen_times_singe >= m_BuHurtList[j].ThroughCount)
                        {
                            continue;
                        }
                        else
                        {
                            m_BulletClass.m_taregt = playerTargetList[i];
                            m_BuHurtList[j].ThroughCount = m_BuHurtList[j].ThroughCount + (Fix64)1;
                        }
                    }
                }
                if (!haveTarget)
                {
                    m_BulletClass.m_taregt = playerTargetList[i];
                    BeHurtTarget target = new BeHurtTarget();
                    target.PlayerTarget = playerTargetList[i];
                    m_BuHurtList.Add(target);
                }
            }
            #endregion
            CreateHitEff(damage);
        }
    }

    //根据目标类型返回穿透总次数限制
    public int ReturnPen_times_max(Player attack, Player target)
    {
        int type = 0;
        if (m_BulletNode == null || m_BulletNode.col_type == null || m_BulletNode.pen_times_max == null)
        {
            return -10;
        }
        bool isthis = false;
        for (int i = 0; i < m_BulletNode.col_type.Length; i++)
        {
            switch ((col_type)m_BulletNode.col_type[i])
            {
                case col_type.self:
                    isthis |= target.m_PlayerData.m_CampId == attack.m_PlayerData.m_CampId && target == attack;
                    break;
                case col_type.selfMonster:
                    isthis |= target.m_PlayerData.m_CampId == attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type != 1;
                    break;
                case col_type.selfHero:
                    isthis |= target.m_PlayerData.m_CampId == attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type == 1;
                    break;
                case col_type.enemyMonster:
                    isthis |= target.m_PlayerData.m_CampId != attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type != 1;
                    break;
                case col_type.enemyHero:
                    isthis |= target.m_PlayerData.m_CampId != attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type == 1;
                    break;
                case col_type.selfTower:
                    break;
                case col_type.enemyTower:
                    break;
                case col_type.neutralMonster:
                    break;
                case col_type.neutralTower:
                    break;
                case col_type.terrain:
                    break;
                case col_type.target:
                    break;
                default:
                    break;
            }
            if (isthis)
            {
                return m_BulletNode.pen_times_max[i];
            }
        }


        return type;
    }

    //加载受击特效
    public void CreateHitEff(float damage)
    {
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            //创建受击特效
            if (!string.IsNullOrEmpty(m_BulletNode.effect_hit) && damage > 0)
            {
                string eff = m_BulletNode.effect_hit;
                if (m_Player.m_PlayerData.m_Type == 1 && !string.IsNullOrEmpty(eff))
                {
                    GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Heros", m_Player.m_PlayerData.m_HeroName, eff));
                    if (effectGo != null)
                        m_AniHitEffect = GameObject.Instantiate(effectGo);
                }
                if (m_Player.m_PlayerData.m_Type == 2 && !string.IsNullOrEmpty(eff))
                {
                    GameObject effectGo = Resources.Load<GameObject>(string.Format("{0}/{1}/{2}/{3}", GameData.m_EffectPath, "Monster", m_Player.m_PlayerData.m_HeroName, eff));
                    if (effectGo != null)
                        m_AniHitEffect = GameObject.Instantiate(effectGo);
                }
                if (m_AniHitEffect == null)
                    return;

                #region 后修改为挂点
                m_AniHitEffect.transform.parent = m_Player.m_VGo.transform;
                #endregion
                m_AniHitEffect.transform.localPosition = Vector3.zero + m_BulletNode.effect_hit_positionxyz;
                m_AniHitEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
                Delay end_delay = new Delay();
                end_delay.InitDestory(m_AniHitEffect, (Fix64)6);
                GameData.m_GameManager.m_DelayManager.m_DelayList.Add(end_delay);
            }
        }
        #endregion
    }

    //检测敌人类型
    public bool CheckHitCondition(BulletNode bulletNode, Player attack, Player target)
    {
        bool result = false;
        if (bulletNode == null)
        {
            return false;
        }
        if (bulletNode.col_type == null)
        {
            return false;
        }
        for (int i = 0; i < bulletNode.col_type.Length; i++)
        {
            switch ((col_type)bulletNode.col_type[i])
            {
                case col_type.self:
                    result |= target.m_PlayerData.m_CampId == attack.m_PlayerData.m_CampId && target == attack;
                    break;
                case col_type.selfMonster:
                    result |= target.m_PlayerData.m_CampId == attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type != 1;
                    break;
                case col_type.selfHero:
                    result |= target.m_PlayerData.m_CampId == attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type == 1;
                    break;
                case col_type.enemyMonster:
                    result |= target.m_PlayerData.m_CampId != attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type != 1;
                    break;
                case col_type.enemyHero:
                    result |= target.m_PlayerData.m_CampId != attack.m_PlayerData.m_CampId && target != attack && target.m_PlayerData.m_Type == 1;
                    break;
                case col_type.selfTower:
                    break;
                case col_type.enemyTower:
                    break;
                case col_type.neutralMonster:
                    break;
                case col_type.neutralTower:
                    break;
                case col_type.terrain:
                    break;
                case col_type.target:
                    break;
                default:
                    break;
            }
        }
        return result;
    }

    //检测子弹目标类型
    public Player CheckBulTargetValue()
    {
        if (m_BulletClass == null)
        {
            return null;
        }
        if (m_BulletClass.m_bul_target_value == (Fix64)1)
        {
            return null;
        }
        if (m_BulletClass.m_bul_target_value == (Fix64)1)
        {
            return m_Player;
        }
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i] == null || GameData.m_PlayerList[i].m_PlayerData == null)
                continue;
            if (m_BulletClass.m_bul_target_value == (Fix64)2)
            {
                if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_Player.m_PlayerData.m_CampId && GameData.m_PlayerList[i] != m_Player && GameData.m_PlayerList[i].m_PlayerData.m_Type != 1 && checkIsInBulTargetSize(GameData.m_PlayerList[i]))
                {
                    return GameData.m_PlayerList[i];
                }
            }
            else if (m_BulletClass.m_bul_target_value == (Fix64)3)
            {
                if (GameData.m_PlayerList[i].m_PlayerData.m_CampId != m_Player.m_PlayerData.m_CampId && GameData.m_PlayerList[i] != m_Player && GameData.m_PlayerList[i].m_PlayerData.m_Type != 1 && checkIsInBulTargetSize(GameData.m_PlayerList[i]))
                {
                    return GameData.m_PlayerList[i];
                }
            }
            else if (m_BulletClass.m_bul_target_value == (Fix64)4)
            {
                if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_Player.m_PlayerData.m_CampId && GameData.m_PlayerList[i] != m_Player && GameData.m_PlayerList[i].m_PlayerData.m_Type == 1 && checkIsInBulTargetSize(GameData.m_PlayerList[i]))
                {
                    return GameData.m_PlayerList[i];
                }
            }
            else if (m_BulletClass.m_bul_target_value == (Fix64)5)
            {
                if (GameData.m_PlayerList[i].m_PlayerData.m_CampId != m_Player.m_PlayerData.m_CampId && GameData.m_PlayerList[i] != m_Player && GameData.m_PlayerList[i].m_PlayerData.m_Type == 1 && checkIsInBulTargetSize(GameData.m_PlayerList[i]))
                {
                    return GameData.m_PlayerList[i];
                }
            }
            else if (m_BulletClass.m_bul_target_value == (Fix64)6)
            {
                if (GameData.m_PlayerList[i] == m_Player)
                {
                    return GameData.m_PlayerList[i];
                }
            }
        }
        return null;
    }
    //检测子弹目标范围参数
    public bool checkIsInBulTargetSize(Player Target)
    {
        Fix64 distance = FixVector3.Distance(Target.m_Pos, m_Pos);
        if (distance <= m_BulletClass.m_bul_target_size)
        {
            return true;
        }
        return false;
    }

    ////检测玩家
    public void GetPlayerRangeList(FixVector3 m_Pos)
    {
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i] == null || GameData.m_PlayerList[i].m_PlayerData == null)
                continue;
            if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_Player.m_PlayerData.m_CampId)
                continue;
            //子弹与敌人的方向向量
            FixVector3 targetV3 = GameData.m_PlayerList[i].m_Pos - m_Pos;
            //求玩家正前方、玩家与敌人方向两个向量的夹角
            Fix64 angle = FixVector3.Angle(m_Angles, targetV3);
            Fix64 distance = FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos);
            if (((float)angle <= m_SkillNode.angle / 2 || m_SkillNode.angle <= 0) && (float)distance <= m_SkillNode.aoe_long && !m_WoundPlayerList.Contains(GameData.m_PlayerList[i]))
            {
                float base_num1 = m_SkillNode.base_num1[0];
                float growth_ratio = m_SkillNode.growth_ratio1[0];
                float skill_ratio = m_SkillNode.stats[0];
                int stats = m_SkillNode.stats[0];
                float attack = m_Player.m_PlayerData.m_HeroAttrNode.attack;
                float armor = GameData.m_PlayerList[i].m_PlayerData.m_HeroAttrNode.armor;
                float attack_hurt = m_Player.m_PlayerData.m_HeroAttrNode.attack_hurt;
                float hurt_addition = m_Player.m_PlayerData.m_HeroAttrNode.hurt_addition;
                float hurt_remission = GameData.m_PlayerList[i].m_PlayerData.m_HeroAttrNode.hurt_remission;
                //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
                int damage = (int)(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission));
                damage = Mathf.Abs(damage);
                GameData.m_PlayerList[i].FallDamage(damage);
                m_WoundPlayerList.Add(GameData.m_PlayerList[i]);
            }
        }
    }
    //检测塔
    public void GetTowerRangeList(FixVector3 m_Pos)
    {
        for (int i = 0; i < GameData.m_TowerList.Count; i++)
        {
            if (GameData.m_TowerList[i] == null || GameData.m_TowerList[i].m_VGo == null)
                continue;
            if (GameData.m_TowerList[i].m_CampId == m_Player.m_PlayerData.m_CampId)
                continue;
            //玩家与敌人的方向向量
            FixVector3 targetV3 = GameData.m_TowerList[i].m_Pos - m_Pos;
            //求玩家正前方、玩家与敌人方向两个向量的夹角
            //这地方求夹角将来要使用定点数或者其他方法换掉，暂时使用Vector3类型
            Fix64 angle = FixVector3.Angle(m_Angles, targetV3);
            Fix64 distance = GameData.m_TowerList[i].m_Type == 1 ? (FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos) - Fix64.FromRaw(500)) : (FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos) - Fix64.One);
            if (((float)angle <= m_SkillNode.angle / 2 || m_SkillNode.angle <= 0) && (float)distance <= m_SkillNode.aoe_long && !m_WoundTowerList.Contains(GameData.m_TowerList[i]))
            {
                float base_num1 = m_SkillNode.base_num1[0];
                float growth_ratio = m_SkillNode.growth_ratio1[0];
                float skill_ratio = m_SkillNode.stats[0];
                int stats = m_SkillNode.stats[0];
                float attack = m_Player.m_PlayerData.m_HeroAttrNode.attack;
                float armor = 0;
                float attack_hurt = m_Player.m_PlayerData.m_HeroAttrNode.attack_hurt;
                float hurt_addition = m_Player.m_PlayerData.m_HeroAttrNode.hurt_addition;
                float hurt_remission = 0;
                //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
                int damage = (int)(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission));
                GameData.m_TowerList[i].FallDamage(damage);
                m_WoundTowerList.Add(GameData.m_TowerList[i]);
            }
        }
    }
    /// <summary>
    /// 创建子子弹
    /// </summary>
    /// <param name="createType"></param>
    /// 1,销毁触发，碰撞触发
    public void CreateSonBullet(int createType, Vector3 targetpos = new Vector3())
    {
        if (m_BulletClass.son_now > m_BulletClass.m_bul_son_max - (Fix64)1)//最大轮数限制
        {
            return;
        }
        if (m_BulletNode.newbul != null && m_BulletNode.newbul.Count > 0)
        {
            int count_temp = 0;
            for (int i = 0; i < m_BulletNode.newbul.Count; i++)
            {
                if (createType != m_BulletNode.newbul[i][0])//符合触发类型时创建
                    return;
                if (i >= m_BulletNode.newbul.Count)
                {
                    //Debug.LogError("     子弹触发时间点数组长度不对       ");
                    return;
                }
                //Debug.LogError(createType + "    创建子子弹" + m_AttackTime);
                if (i == 0)
                {
                    m_BulletClass.son_now = m_BulletClass.son_now + (Fix64)1;
                }
                double time = 0;
                if (m_BulletNode.newbul_dalay.Count > i && m_BulletNode.newbul_dalay[i] != null)
                {
                    time = m_BulletNode.newbul_dalay[i][0];
                }
                Delay delay = new Delay();
                delay.DelayDo((Fix64)time, () =>
                {
                    ////Debug.LogError("触发轮数：" + m_BulletClass.son_now);
                    BaseBullet m_SkillState = new BaseBullet();
                    Bullet_ValueClass bullet = m_BulletClass;
                    bullet.m_BulletIndex = (Fix64)count_temp;
                    if (m_BulletNode.newbul.Count > 0 && m_BulletNode.newbul[count_temp] != null)
                    {
                        bullet.m_BulletId = (Fix64)m_BulletNode.newbul[count_temp][1];
                    }
                    else
                    {
                        //Debug.LogError("newbul");
                    }
                    //------------
                    if (m_BulletNode.newbul_target_type[count_temp] != null && m_BulletNode.newbul_target_type[count_temp].Length > 0)
                    {
                        bullet.m_bul_target_type = (Fix64)m_BulletNode.newbul_target_type[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_target_type");
                    }
                    //------------
                    if (m_BulletNode.newbul_target_value.Count > 0 && m_BulletNode.newbul_target_value[count_temp] != null && m_BulletNode.newbul_target_value[count_temp].Length > 0)
                    {
                        //bullet.m_bul_target_value = new Fix64[m_BulletNode.newbul_target_value[count_temp].Length];
                        //for (int j = 0; j < m_BulletNode.newbul_target_value[count_temp].Length; j++)
                        //{
                        //    bullet.m_bul_target_value[j] = (Fix64)m_BulletNode.newbul_target_value[count_temp][j];
                        //}
                        bullet.m_bul_target_value = (Fix64)m_BulletNode.newbul_target_value[count_temp][0];

                    }
                    else
                    {
                        //Debug.LogError("newbul_target_value");
                    }
                    //------------
                    if (m_BulletNode.newbul_target_size.Count > 0 && m_BulletNode.newbul_target_size[count_temp] != null && m_BulletNode.newbul_target_size[count_temp].Length > 0)
                    {
                        bullet.m_bul_target_size = (Fix64)m_BulletNode.newbul_target_size[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_target_size");

                    }
                    //------------
                    if (m_BulletNode.newbul_firing.Count > 0 && m_BulletNode.newbul_firing[count_temp] != null && m_BulletNode.newbul_firing[count_temp].Length > 0)
                    {
                        bullet.m_bul_start = (Fix64)m_BulletNode.newbul_firing[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_firing");
                    }
                    //------------
                    if (m_BulletNode.newbul_firing_xyz.Count > 0 && m_BulletNode.newbul_firing_xyz[count_temp] != null)
                    {
                        bullet.m_firing_xyz = (FixVector3)m_BulletNode.newbul_firing_xyz[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("newbul_firing_xyz");
                    }
                    //------------
                    if (m_BulletNode.newbul_end.Count > 0 && m_BulletNode.newbul_end[count_temp] != null && m_BulletNode.newbul_end[count_temp].Length > 0)
                    {
                        bullet.m_bul_end = (Fix64)m_BulletNode.newbul_end[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_end");
                    }
                    //------------
                    if (m_BulletNode.newbul_end_xyz.Count > 0 && m_BulletNode.newbul_end_xyz[count_temp] != null)
                    {
                        bullet.m_bul_end_xyz = (FixVector3)m_BulletNode.newbul_end_xyz[count_temp];
                    }
                    else
                    {
                        //Debug.LogError("newbul_end_xyz");
                    }
                    //------------
                    if (m_BulletNode.newbul_angle.Count > 0 && m_BulletNode.newbul_angle[count_temp] != null && m_BulletNode.newbul_angle[count_temp].Length > 0)
                    {
                        bullet.m_bul_end_angle = (Fix64)m_BulletNode.newbul_angle[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_angle");
                    }
                    //------------
                    if (m_BulletNode.newbul_origin.Count > 0 && m_BulletNode.newbul_origin[count_temp] != null && m_BulletNode.newbul_origin[count_temp].Length > 0)
                    {
                        bullet.newbul_origin = (Fix64)m_BulletNode.newbul_origin[count_temp][0];
                        if (bullet.newbul_origin == (Fix64)1)
                        {
                            bullet.old_pos = (FixVector3)targetpos;
                        }
                        else if (bullet.newbul_origin == (Fix64)3)
                        {
                            bullet.old_pos = m_Pos;
                        }
                    }
                    else
                    {
                        //Debug.LogError("newbul_origin");
                    }
                    //------------
                    if (m_BulletNode.newbul_target_extra.Count > 0 && m_BulletNode.newbul_target_extra[count_temp] != null && m_BulletNode.newbul_target_extra[count_temp].Length > 0)
                    {
                        bullet.newbul_target_extra = (Fix64)m_BulletNode.newbul_target_extra[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_target_extra");
                    }
                    //------------
                    if (m_BulletNode.newbul_max.Count > 0 && m_BulletNode.newbul_max[count_temp] != null && m_BulletNode.newbul_max[count_temp].Length > 0)
                    {
                        bullet.m_max_bul = (Fix64)m_BulletNode.newbul_max[count_temp][0];
                    }
                    else
                    {
                        //Debug.LogError("newbul_target_extra");
                    }
                    //------------
                    if (m_BulletNode.newbul_num_single.Count > 0 && m_BulletNode.newbul_num_single[count_temp] != null && m_BulletNode.newbul_num_single[count_temp].Length > 0)
                    {
                        bullet.newbul_num_single = new Fix64[m_BulletNode.newbul_num_single[count_temp].Length];
                        for (int j = 0; j < m_BulletNode.newbul_num_single[count_temp].Length; j++)
                        {
                            bullet.newbul_num_single[j] = (Fix64)m_BulletNode.newbul_num_single[count_temp][j];
                        }
                    }
                    else
                    {
                        //Debug.LogError("newbul_num_single");
                    }
                    //switch (m_Player.m_SkillNode.skill_usetype)
                    //{
                    //    case SkillUseType.None:
                    //        break;
                    //    case SkillUseType.Direction://方向型
                    //        bullet.v_pos = (FixVector3)(m_Player.m_Angles.ToVector3().normalized * 10);

                    //        break;
                    //    case SkillUseType.Point://坐标点型
                    //        bullet.v_pos = (FixVector3)m_Player.m_TargetPlayer.m_Pos;

                    //        break;
                    //    case SkillUseType.Forward://直接释放型
                    //        bullet.v_pos = (FixVector3)(m_Player.m_Angles.ToVector3().normalized * 10);
                    //        break;
                    //    case SkillUseType.Target://目标型
                    //        bullet.v_taregt.Add(m_Player.m_TargetPlayer);
                    //        bullet.m_taregt = m_Player.m_TargetPlayer;
                    //        break;
                    //    default:
                    //        break;
                    //}
                    for (int j = 0; j < (int)bullet.m_max_bul; j++)
                    {
                        m_SkillState.CreateBullet(m_Player, bullet, m_Parameter);
                        m_SkillState.OnEnter();
                        GameData.m_GameManager.m_BulletManager.m_AttackList.Add(m_SkillState);
                    }
                    count_temp++;
                });
                GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
            }

        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destory()
    {
        //Delay delay = new Delay();
        //delay.InitDestory(m_AniEffect, (Fix64)2);
        //GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
    }
}
