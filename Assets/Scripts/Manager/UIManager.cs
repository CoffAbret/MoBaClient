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
    public delegate void UpdateMatchTimeUICallback(int time);
    public UpdateMatchTimeUICallback m_UpdateMatchTimeUICallback;
    //游戏匹配成功UI
    public delegate void UpdateMatchSuccessUICallback();
    public UpdateMatchSuccessUICallback m_UpdateMatchSuccessUICallback;
    //确认进入匹配UI
    public delegate void UpdateConfirmMatchUICallback(int campId,int pos);
    public UpdateConfirmMatchUICallback m_UpdateConfirmMatchUICallback;
    //进入选择英雄UI
    public delegate void UpdateMatchHeroRoomUICallback();
    public UpdateMatchHeroRoomUICallback m_UpdateMatchHeroRoomUICallback;
    //进入游戏战斗UI
    public delegate void UpdateEmbattleUICallback();
    public UpdateEmbattleUICallback m_UpdateEmbattleUICallback;
    //刷新技能图标
    public delegate void UpdateSkillUICallback(List<SkillNode> skillNodeList);
    public UpdateSkillUICallback m_UpdateSkillUICallback = null;

    //刷新技能CD
    public delegate void UpdateSkillCDUICallback(int cdTime, int index);
    public UpdateSkillCDUICallback m_UpdateSkillCDUICallback = null;

    //刷新主角死亡UI
    public delegate void UpdatePlayerDieUICallback(bool isDie);
    public UpdatePlayerDieUICallback m_UpdatePlayerDieUICallback = null;
    //当前角色死亡倒计时
    public UILabel m_ResurrectionLabel;

    //刷新敌方死亡UI
    public delegate void UpdateEnemyDieUICallback(bool isDie, PlayerData data);
    public UpdateEnemyDieUICallback m_UpdateEnemyDieUICallback = null;
    //敌方角色死亡倒计时
    public UILabel m_EnemyResurrectionLabel;

    public delegate void UpdateAddHpCallback(Player player);
    public UpdateAddHpCallback m_UpdateAddHpCallback;

    //游戏结束UI
    public delegate void GameOverUICallback();
    public GameOverUICallback m_GameOverUICallback;

}
