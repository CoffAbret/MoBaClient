using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI对象管理类
/// </summary>
public class UIManager
{
    //刷新技能图标
    public delegate void UpdateSkillUICallback(List<SkillNode> skillNodeList);
    public UpdateSkillUICallback m_UpdateSkillUICallback = null;

    //刷新主角死亡UI
    public delegate void UpdatePlayerDieUICallback(bool isDie);
    public UpdatePlayerDieUICallback m_UpdatePlayerDieUICallback = null;
    //当前角色死亡倒计时
    public UILabel m_ResurrectionLabel;

    //刷新敌方死亡UI
    public delegate void UpdateEnemyDieUICallback(bool isDie,PlayerData data);
    public UpdateEnemyDieUICallback m_UpdateEnemyDieUICallback = null;
    //敌方角色死亡倒计时
    public UILabel m_EnemyResurrectionLabel;
}
