//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 子弹
///// </summary>
//public class PlayerBullet
//{
//    public string m_PlayerAttackId = string.Empty;
//    //位置
//    public FixVector3 m_Pos = FixVector3.Zero;
//    //朝向
//    public FixVector3 m_Angles = FixVector3.Zero;
//    //攻击间隔
//    public Fix64 m_AttackSpeed = Fix64.FromRaw(100);
//    //攻击者
//    public Player m_AttackPlayer;
//    //攻击技能
//    public SkillNode m_SkillNode;
//    //子弹是否有效
//    public bool m_IsActive = false;
//    //受伤角色列表
//    public List<Player> m_WoundPlayerList;
//    //受伤箭塔列表
//    public List<Tower> m_WoundTowerList;
//    //累计时间
//    private Fix64 m_AttackTime;
//    /// <summary>
//    /// 创建对象
//    /// </summary>
//    /// <param name="charData">对象数据</param>
//    public void Create(Player player, SkillNode node)
//    {
//        m_PlayerAttackId = Guid.NewGuid().ToString();
//        m_IsActive = true;
//        m_AttackPlayer = player;
//        m_SkillNode = node;
//        m_Pos = player.m_Pos;
//        m_Angles = (FixVector3)(player.m_VGo.transform.rotation * Vector3.forward);
//        m_WoundPlayerList = new List<Player>();
//        m_WoundTowerList = new List<Tower>();
//    }

//    /// <summary>
//    /// 遍历状态
//    /// </summary>
//    public void UpdateLogic()
//    {
//        if (m_AttackPlayer == null || m_AttackPlayer.m_PlayerData == null)
//            return;
//        if (m_AttackTime > (Fix64)m_SkillNode.max_fly)
//        {
//            m_IsActive = false;
//            return;
//        }

//        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
//        {
//            if (GameData.m_PlayerList[i] == null || GameData.m_PlayerList[i].m_PlayerData == null)
//                continue;
//            if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_AttackPlayer.m_PlayerData.m_CampId)
//                continue;
//            if (m_WoundPlayerList.Contains(GameData.m_PlayerList[i]))
//                continue;
//            //子弹与敌人的方向向量
//            FixVector3 targetFixVec = GameData.m_PlayerList[i].m_Pos - m_Pos;
//            //求玩家正前方、玩家与敌人方向两个向量的夹角
//            bool IsFallDamage = false;
//            if (m_SkillNode.aoe_wide <= 0)
//            {
//                Fix64 angle = (Fix64)(Mathf.Acos((float)FixVector3.Dot(m_Angles.GetNormalized(), targetFixVec.GetNormalized()))) * (Fix64)Mathf.Rad2Deg;
//                Fix64 compareAngle = (Fix64)m_SkillNode.angle;
//                compareAngle = compareAngle / Fix64.FromRaw(2000);
//                Fix64 compareDistance = (Fix64)m_SkillNode.aoe_long;
//                GameData.m_GameManager.LogMsg(string.Format("角度:{0}", angle));
//                GameData.m_GameManager.LogMsg(string.Format("配置表角度:{0}", compareAngle));
//                Fix64 distance = GameData.m_PlayerList[i].m_PlayerData.m_Type == 1 ? (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(200)) : (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(100));
//                GameData.m_GameManager.LogMsg(string.Format("距离:{0}", distance));
//                GameData.m_GameManager.LogMsg(string.Format("配置表距离:{0}", compareDistance));
//                if ((angle <= compareAngle || compareAngle <= Fix64.Zero) && (distance <= compareDistance))
//                {
//                    IsFallDamage = true;
//                }
//            }
//            else
//            {
//                Fix64 forwardDistance = FixVector3.Dot(targetFixVec.GetNormalized(), m_Angles);
//                Fix64 compareDistance = (Fix64)m_SkillNode.aoe_long;
//                Fix64 distance = GameData.m_PlayerList[i].m_PlayerData.m_Type == 1 ? (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(200)) : (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(100));
//                GameData.m_GameManager.LogMsg(string.Format("往前距离:{0}", forwardDistance));
//                GameData.m_GameManager.LogMsg(string.Format("配置表往前距离:{0}", compareDistance));
//                if (forwardDistance > Fix64.Zero && forwardDistance <= compareDistance && distance < compareDistance)
//                {
//                    Fix64 rightDistance = FixVector3.Dot(targetFixVec, (FixVector3)m_AttackPlayer.m_VGo.transform.right.normalized);
//                    rightDistance = rightDistance < Fix64.Zero ? -rightDistance : rightDistance;
//                    Fix64 compareWide = (Fix64)m_SkillNode.aoe_wide;
//                    GameData.m_GameManager.LogMsg(string.Format("往右距离:{0}", rightDistance));
//                    GameData.m_GameManager.LogMsg(string.Format("配置表往右距离:{0}", compareWide));
//                    if (rightDistance <= compareWide)
//                    {
//                        IsFallDamage = true;
//                    }
//                }
//            }
//            if (IsFallDamage)
//            {
//                float base_num1 = m_SkillNode.base_num1[0];
//                float growth_ratio = m_SkillNode.growth_ratio1[0];
//                float skill_ratio = m_SkillNode.skill_ratio[0];
//                int stats = m_SkillNode.stats[0];
//                float attack = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack;
//                float armor = GameData.m_PlayerList[i].m_PlayerData.m_HeroAttrNode.armor;
//                float attack_hurt = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack_hurt;
//                float hurt_addition = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.hurt_addition;
//                float hurt_remission = GameData.m_PlayerList[i].m_PlayerData.m_HeroAttrNode.hurt_remission;
//                //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
//                int damage = (int)Math.Ceiling(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission));
//                damage = Mathf.Abs(damage);
//                GameData.m_PlayerList[i].FallDamage(damage);
//                m_WoundPlayerList.Add(GameData.m_PlayerList[i]);
//            }
//        }

//        for (int i = 0; i < GameData.m_TowerList.Count; i++)
//        {
//            if (GameData.m_TowerList[i] == null || GameData.m_TowerList[i].m_VGo == null)
//                continue;
//            if (GameData.m_TowerList[i].m_CampId == m_AttackPlayer.m_PlayerData.m_CampId)
//                continue;
//            if (m_WoundTowerList.Contains(GameData.m_TowerList[i]))
//                continue;
//            //子弹与敌人的方向向量
//            FixVector3 targetFixVec = GameData.m_TowerList[i].m_Pos - m_Pos;
//            //求玩家正前方、玩家与敌人方向两个向量的夹角
//            bool IsFallDamage = false;
//            if (m_SkillNode.aoe_wide <= 0)
//            {
//                Fix64 angle = (Fix64)(Mathf.Acos((float)FixVector3.Dot(m_Angles.GetNormalized(), targetFixVec.GetNormalized()))) * (Fix64)Mathf.Rad2Deg;
//                Fix64 compareAngle = (Fix64)m_SkillNode.angle;
//                compareAngle = compareAngle / Fix64.FromRaw(2000);
//                Fix64 compareDistance = (Fix64)m_SkillNode.aoe_long;
//                GameData.m_GameManager.LogMsg(string.Format("角度:{0}", angle));
//                GameData.m_GameManager.LogMsg(string.Format("配置表角度:{0}", compareAngle));
//                Fix64 distance = GameData.m_TowerList[i].m_Type == 1 ? (FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos) - Fix64.FromRaw(500)) : (FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos) - Fix64.One);
//                GameData.m_GameManager.LogMsg(string.Format("距离:{0}", distance));
//                GameData.m_GameManager.LogMsg(string.Format("配置表距离:{0}", compareDistance));
//                if ((angle <= compareAngle || compareAngle <= Fix64.Zero) && (distance <= compareDistance))
//                {
//                    IsFallDamage = true;
//                }
//            }
//            else
//            {
//                Fix64 forwardDistance = FixVector3.Dot(targetFixVec.GetNormalized(), m_Angles);
//                Fix64 compareDistance = (Fix64)m_SkillNode.aoe_long;
//                Fix64 distance = GameData.m_PlayerList[i].m_PlayerData.m_Type == 1 ? (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(500)) : (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.One);
//                GameData.m_GameManager.LogMsg(string.Format("往前距离:{0}", forwardDistance));
//                GameData.m_GameManager.LogMsg(string.Format("配置表往前距离:{0}", compareDistance));
//                if (forwardDistance > Fix64.Zero && forwardDistance <= compareDistance && distance < compareDistance)
//                {
//                    Fix64 rightDistance = FixVector3.Dot(targetFixVec, (FixVector3)m_AttackPlayer.m_VGo.transform.right.normalized);
//                    rightDistance = rightDistance < Fix64.Zero ? -rightDistance : rightDistance;
//                    Fix64 compareWide = (Fix64)m_SkillNode.aoe_wide;
//                    GameData.m_GameManager.LogMsg(string.Format("往右距离:{0}", rightDistance));
//                    GameData.m_GameManager.LogMsg(string.Format("配置表往右距离:{0}", compareWide));
//                    if (rightDistance <= compareWide)
//                    {
//                        IsFallDamage = true;
//                    }
//                }
//            }
//            if (IsFallDamage)
//            {
//                float base_num1 = m_SkillNode.base_num1[0];
//                float growth_ratio = m_SkillNode.growth_ratio1[0];
//                float skill_ratio = m_SkillNode.skill_ratio[0];
//                int stats = m_SkillNode.stats[0];
//                float attack = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack;
//                float armor = 0;
//                float attack_hurt = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack_hurt;
//                float hurt_addition = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.hurt_addition;
//                float hurt_remission = 0;
//                //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
//                int damage = (int)Math.Ceiling(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission));
//                damage = Mathf.Abs(damage);
//                GameData.m_TowerList[i].FallDamage(damage);
//                m_WoundTowerList.Add(GameData.m_TowerList[i]);
//            }
//        }
//        m_Pos = m_Pos + m_Angles * (Fix64)m_SkillNode.flight_speed * GameData.m_FixFrameLen;
//        m_AttackTime += GameData.m_FixFrameLen;
//    }

//    /// <summary>
//    /// 销毁
//    /// </summary>
//    public void Destory()
//    {
//        m_AttackPlayer = null;
//        m_SkillNode = null;
//        m_WoundPlayerList.Clear();
//        m_WoundPlayerList = null;
//    }
//}
