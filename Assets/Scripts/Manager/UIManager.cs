using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI对象管理类
/// </summary>
public class UIManager
{

    //游戏匹配UI
    public delegate void UpdateMatchUICallback();
    public UpdateMatchUICallback m_UpdateMatchUICallback;
    //游戏匹配倒计时UI
    public delegate void UpdateMatchCountdownUICallback(int time);
    public UpdateMatchCountdownUICallback m_UpdateMatchCountdownUICallback;
    //游戏匹配成功UI
    public delegate void UpdateMatchSuccessUICallback();
    public UpdateMatchSuccessUICallback m_UpdateMatchSuccessUICallback;
    //确认进入匹配UI
    public delegate void UpdateSelectHeroConfirmMatchUICallback(int campId, int pos);
    public UpdateSelectHeroConfirmMatchUICallback m_UpdateSelectHeroConfirmMatchUI;
    //进入选择英雄UI
    public delegate void UpdateSelectHeroUI();
    public UpdateSelectHeroUI m_UpdateSelectHeroUI;
    //进入游戏战斗UI
    public delegate void UpdateBattleUICallback();
    public UpdateBattleUICallback m_UpdateBattleUICallback;
    //刷新技能图标
    public delegate void UpdateBattleSkillUICallback(List<SkillNode> skillNodeList);
    public UpdateBattleSkillUICallback m_UpdateBattleSkillUICallback = null;
    //刷新技能CD
    public delegate void UpdateBattleSkillCDUICallback(int cdTime, int index);
    public UpdateBattleSkillCDUICallback m_UpdateBattleSkillCDUICallback = null;
    //刷新主角死亡UI
    public delegate void UpdateResurrectionCountdownUICallback(bool isDie);
    public UpdateResurrectionCountdownUICallback m_UpdateResurrectionCountdownUICallback = null;
    //当前角色死亡倒计时
    public UILabel m_ResurrectionCountdown;
    //刷新敌方死亡UI
    public delegate void UpdateEnemyDieUICallback(bool isDie, PlayerData data);
    public UpdateEnemyDieUICallback m_UpdateEnemyResurrectionCountdownUICallback = null;
    //敌方角色死亡倒计时
    public UILabel m_EmenyResurrectionCountdown;
    public delegate void UpdateAddHpCallback(Player player);
    public UpdateAddHpCallback m_UpdateAddHpCallback;
    //游戏结算UI
    public delegate void SettlementUICallback(bool result);
    public SettlementUICallback m_SettlementUICallback;

}
