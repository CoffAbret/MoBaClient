using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class MainBehaviour : MonoBehaviour
{
    //普攻按钮
    public GameObject m_AttackGo;
    //技能一
    public GameObject m_Skill1Go;
    //技能二
    public GameObject m_Skill2Go;
    //技能三
    public GameObject m_Skill3Go;
    //技能四
    public GameObject m_Skill4Go;
    //主界面UI
    public GameObject m_MainUIGo;
    //复活界面UI
    public GameObject m_ResurrectionUIGo;
    public UILabel m_ResurrectionLabel;
    //敌方复活UI
    public GameObject m_EnemyResurrectionUIGo;
    public UILabel m_EnemyResurrectionLabel;
    public UISprite m_EnemyResurrectionSprite;
    //选择英雄UI
    public GameObject m_UIEmbattleUIGo;
    public GameObject m_ExitGame;
    public GameObject m_Hero1;
    public GameObject m_Hero1Selected;
    public GameObject m_Hero2;
    public GameObject m_Hero2Selected;
    public UILabel m_IPTxt;
    public UILabel m_PortTxt;
    public GameObject m_JoinGame;
    //小地图UI
    public GameObject m_MiniMapUI;
    //游戏结束UI
    public GameObject m_UITheBattleUI;
    //技能索引
    private int m_Index = 0;
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        //修改每帧时间长度
        Time.timeScale = 1f;
        //允许后台运行
        Application.runInBackground = true;
        //固定50帧
        Application.targetFrameRate = 50;
    }
    /// <summary>
    /// 开始准备游戏数据以及网络连接
    /// 初始化UI
    /// </summary>
    private void Start()
    {
        UIEventListener.Get(m_AttackGo).onClick = OnAttackClick;
        UIEventListener.Get(m_Skill1Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill2Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill3Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill4Go).onClick = OnSkillClick;
        UIEventListener.Get(m_ExitGame).onClick = OnExitGameClick;
        UIEventListener.Get(m_JoinGame).onClick = OnJoinGameClick;
        UIEventListener.Get(m_Hero1).onClick = OnSelectedHeroClick;
        UIEventListener.Get(m_Hero2).onClick = OnSelectedHeroClick;
    }

    /// <summary>
    /// 每帧更新游戏逻辑
    /// </summary>
    private void FixedUpdate()
    {
        if (GameData.m_GameManager == null)
            return;
        GameData.m_GameManager.UpdateGame();
    }

    /// <summary>
    /// 销毁游戏数据
    /// </summary>
    private void OnDestroy()
    {
        if (GameData.m_GameManager == null)
            return;
        GameData.m_GameManager.DestoryGame();
    }

    /// <summary>
    /// 点击普攻
    /// </summary>
    /// <param name="go"></param>
    private void OnAttackClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsSkill)
            return;
        if (GameData.m_CurrentPlayer.m_IsDie)
            return;
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        GameData.m_CurrentPlayer.m_IsAttack = true;
        GameData.m_GameManager.InputCmd(Cmd.Attack);
    }

    /// <summary>
    /// 点击技能
    /// </summary>
    /// <param name="go"></param>
    private void OnSkillClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsSkill)
            return;
        if (GameData.m_CurrentPlayer.m_IsAttack)
            return;
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        if (!int.TryParse(go.name.Substring(go.name.Length - 1, 1), out m_Index))
            return;
        GameData.m_CurrentPlayer.m_IsSkill = true;
        GameData.m_GameManager.InputCmd(Cmd.UseSkill, m_Index.ToString());
    }

    private void OnExitGameClick(GameObject go)
    {
        Application.Quit();
    }
    private void OnJoinGameClick(GameObject go)
    {
        int port;
        GameData.m_IP = m_IPTxt.text.Trim();
        int.TryParse(m_PortTxt.text.Trim(), out port);
        GameData.m_Port = port;
        m_UIEmbattleUIGo.SetActive(false);
        m_MiniMapUI.SetActive(true);
        m_MainUIGo.SetActive(true);
        GameData.m_GameManager = new GameManager();
        GameData.m_GameManager.InitGame();
        GameData.m_GameManager.InputReady();
        GameData.m_GameManager.m_UIManager.m_UpdateSkillUICallback = OnUpdateSkillUI;

        GameData.m_GameManager.m_UIManager.m_UpdatePlayerDieUICallback = OnUpdatePlayerDieUI;
        GameData.m_GameManager.m_UIManager.m_ResurrectionLabel = m_ResurrectionLabel;

        GameData.m_GameManager.m_UIManager.m_UpdateEnemyDieUICallback = OnUpdateEnemyResurrectionUI;
        GameData.m_GameManager.m_UIManager.m_EnemyResurrectionLabel = m_EnemyResurrectionLabel;
        GameData.m_GameManager.m_UIManager.m_GameOverUICallback = OnGameOverUI;
    }
    private void OnSelectedHeroClick(GameObject go)
    {
        if (go.name.Contains("1"))
        {
            GameData.m_HeroId = 201001000;
            m_Hero1Selected.SetActive(true);
            m_Hero2Selected.SetActive(false);
        }
        else
        {
            GameData.m_HeroId = 201003300;
            m_Hero2Selected.SetActive(true);
            m_Hero1Selected.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新当前角色技能UI
    /// </summary>
    /// <param name="skillNodeList"></param>
    private void OnUpdateSkillUI(List<SkillNode> skillNodeList)
    {
        m_Skill1Go.GetComponent<UISprite>().spriteName = skillNodeList[3].skill_icon;
        m_Skill2Go.GetComponent<UISprite>().spriteName = skillNodeList[4].skill_icon;
        m_Skill3Go.GetComponent<UISprite>().spriteName = skillNodeList[5].skill_icon;
        m_Skill4Go.GetComponent<UISprite>().spriteName = skillNodeList[6].skill_icon;

        m_Skill1Go.GetComponent<UIButton>().normalSprite = skillNodeList[3].skill_icon;
        m_Skill2Go.GetComponent<UIButton>().normalSprite = skillNodeList[4].skill_icon;
        m_Skill3Go.GetComponent<UIButton>().normalSprite = skillNodeList[5].skill_icon;
        m_Skill4Go.GetComponent<UIButton>().normalSprite = skillNodeList[6].skill_icon;
    }

    /// <summary>
    /// 刷新当前角色死亡UI
    /// </summary>
    /// <param name="skillNodeList"></param>
    private void OnUpdatePlayerDieUI(bool isDie)
    {
        if (isDie)
        {
            m_MainUIGo.SetActive(false);
            m_ResurrectionUIGo.SetActive(true);
            m_ResurrectionLabel.text = "";
        }
        else
        {
            m_MainUIGo.SetActive(true);
            m_ResurrectionUIGo.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新敌方死亡UI
    /// </summary>
    /// <param name="isDie"></param>
    private void OnUpdateEnemyResurrectionUI(bool isDie, PlayerData data)
    {
        if (isDie)
        {
            m_EnemyResurrectionUIGo.SetActive(true);
            m_EnemyResurrectionLabel.text = "";
            m_EnemyResurrectionSprite.spriteName = data.m_HeroAttrNode.icon_name;
        }
        else
        {
            m_EnemyResurrectionUIGo.SetActive(false);
        }
    }

    /// <summary>
    /// 游戏结束UI
    /// </summary>
    private void OnGameOverUI()
    {
        m_MainUIGo.SetActive(false);
        m_UITheBattleUI.SetActive(true);
    }
}
