using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色子弹
/// </summary>
public class PlayerAttack
{
    //位置
    public FixVector3 m_Pos = FixVector3.Zero;
    //朝向
    public FixVector3 m_Angles = FixVector3.Zero;
    //攻击间隔
    public Fix64 m_AttackSpeed = Fix64.FromRaw(100);
    //攻击者
    public Player m_AttackPlayer;
    //攻击技能
    public SkillNode m_SkillNode;
    //子弹是否有效
    public bool m_IsActive = false;
    //受伤角色列表
    public List<Player> m_WoundPlayerList;
    //受伤箭塔列表
    public List<Tower> m_WoundTowerList;
    //累计时间
    private Fix64 m_AttackTime;
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(Player player, SkillNode node)
    {
        m_IsActive = true;
        m_AttackPlayer = player;
        m_SkillNode = node;
        m_Pos = player.m_Pos;
        m_Angles = player.m_Angles;
        m_WoundPlayerList = new List<Player>();
        m_WoundTowerList = new List<Tower>();
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_AttackPlayer == null || m_AttackPlayer.m_PlayerData == null)
            return;
        if (m_AttackTime >= (Fix64)m_SkillNode.efficiency_time)
        {
            m_IsActive = false;
            return;
        }
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i] == null || GameData.m_PlayerList[i].m_PlayerData == null)
                continue;
            if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_AttackPlayer.m_PlayerData.m_CampId)
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
                float attack = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack;
                float armor = GameData.m_PlayerList[i].m_PlayerData.m_HeroAttrNode.armor;
                float attack_hurt = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack_hurt;
                float hurt_addition = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.hurt_addition;
                float hurt_remission = GameData.m_PlayerList[i].m_PlayerData.m_HeroAttrNode.hurt_remission;
                //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
                int damage = (int)(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission));
                damage = Mathf.Abs(damage);
                GameData.m_PlayerList[i].FallDamage(damage);
                m_WoundPlayerList.Add(GameData.m_PlayerList[i]);
            }
        }

        for (int i = 0; i < GameData.m_TowerList.Count; i++)
        {
            if (GameData.m_TowerList[i] == null || GameData.m_TowerList[i].m_VGo == null)
                continue;
            if (GameData.m_TowerList[i].m_CampId == m_AttackPlayer.m_PlayerData.m_CampId)
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
                float attack = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack;
                float armor = 0;
                float attack_hurt = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.attack_hurt;
                float hurt_addition = m_AttackPlayer.m_PlayerData.m_HeroAttrNode.hurt_addition;
                float hurt_remission = 0;
                //物理伤害 =（攻方base_num1 + 攻方growth_ratio1 * 1 + 攻方skill_ratio * [if 攻方stats=3 攻方attack else 0] ) * (1 - 守方armor / ( 守方armor * 0.5 + 125)) * 攻方暴击 * 守方闪避 * ( 1 + 攻方attack_hurt） * （1 + 攻方hurt_addition - 守方hurt_remission）
                int damage = (int)(base_num1 + growth_ratio * 1 + skill_ratio * (stats == 3 ? attack : 0) * (1 - armor / (armor * 0.5 + 125)) * 1 * 1 * (1 + attack_hurt) * (1 + hurt_addition - hurt_remission));
                GameData.m_TowerList[i].FallDamage(damage);
                m_WoundTowerList.Add(GameData.m_TowerList[i]);
            }
        }
        m_Pos = m_Pos + m_Angles * (Fix64)m_SkillNode.flight_speed * GameData.m_FixFrameLen;
        m_AttackTime += GameData.m_FixFrameLen;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destory()
    {
        m_AttackPlayer = null;
        m_SkillNode = null;
        m_WoundPlayerList.Clear();
        m_WoundPlayerList = null;
    }
}
