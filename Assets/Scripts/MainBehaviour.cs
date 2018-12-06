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
    public UISprite m_Skill1CD;
    //技能二
    public GameObject m_Skill2Go;
    public UISprite m_Skill2CD;
    //技能三
    public GameObject m_Skill3Go;
    public UISprite m_Skill3CD;
    //技能四
    public GameObject m_Skill4Go;
    public UISprite m_Skill4CD;
    //加血技能
    public GameObject m_Skill5Go;
    public UISprite m_Skill5CD;
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
    //退出游戏
    public GameObject m_ExitGame;
    //选择英雄列表
    public List<GameObject> m_HeroItemArray;
    //IP
    public UILabel m_IPTxt;
    //端口
    public UILabel m_PortTxt;
    public GameObject m_JoinGame;
    //小地图UI
    public GameObject m_MiniMapUI;
    //游戏结束UI
    public GameObject m_UITheBattleUI;
    //红方加血点
    public GameObject m_RedAddHp;
    //蓝方加血点
    public GameObject m_BlueAddHp;
    //红方复活点
    public GameObject m_RedPos;
    //蓝方复活点
    public GameObject m_BluePos;
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
        UIEventListener.Get(m_Skill5Go).onClick = OnAddHpSkillClick;
        UIEventListener.Get(m_ExitGame).onClick = OnExitGameClick;
        UIEventListener.Get(m_JoinGame).onClick = OnJoinGameClick;
        for (int i = 0; i < m_HeroItemArray.Count; i++)
            UIEventListener.Get(m_HeroItemArray[i]).onClick = OnSelectedHeroClick;
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

    //普攻切换时间
    private Fix64 m_AttackTime = Fix64.FromRaw(800);
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
        if (!GameData.m_CurrentPlayer.m_IsAttack)
            GameData.m_AttackClickIndex = 0;
        if (GameData.m_AttackClickIndex > 0)
        {
            SkillNode skillNode = GameData.m_CurrentPlayer.m_PlayerData.GetSkillNode(GameData.m_AttackClickIndex);
            if (GameData.m_CurrentPlayer.m_IntervalTime < ((Fix64)skillNode.animatorTime * m_AttackTime))
                return;
        }
        if (GameData.m_AttackClickIndex == 0)
            GameData.m_AttackClickIndex = 1;
        //普攻状态并且是第一个连击并且动作已经播放结束
        else if (GameData.m_AttackClickIndex == 1)
            GameData.m_AttackClickIndex = 2;
        //普攻状态并且是第二个连击并且动作已经播放结束
        else if (GameData.m_AttackClickIndex == 2)
            GameData.m_AttackClickIndex = 3;
        //普攻状态并且是第三个连击并且动作已经播放结束
        else if (GameData.m_AttackClickIndex == 3)
            GameData.m_AttackClickIndex = 1;
        if (GameData.m_AttackClickIndex > 0)
        {
            GameData.m_CurrentPlayer.m_IsAttack = true;
            GameData.m_GameManager.InputCmd(Cmd.Attack, GameData.m_AttackClickIndex.ToString());
        }
    }

    /// <summary>
    /// 点击技能
    /// </summary>
    /// <param name="go"></param>
    private void OnSkillClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        if (GameData.m_CurrentPlayer.m_IsDie)
            return;
        if (!int.TryParse(go.name.Substring(go.name.Length - 1, 1), out m_Index))
            return;
        if (m_Index == 4 && m_Skill1CD.fillAmount > 0)
            return;
        if (m_Index == 5 && m_Skill2CD.fillAmount > 0)
            return;
        if (m_Index == 6 && m_Skill3CD.fillAmount > 0)
            return;
        if (m_Index == 7 && m_Skill4CD.fillAmount > 0)
            return;
        GameData.m_CurrentPlayer.m_IsSkill = true;
        GameData.m_GameManager.InputCmd(Cmd.UseSkill, m_Index.ToString());
    }

    /// <summary>
    /// 点击技能
    /// </summary>
    /// <param name="go"></param>
    private void OnAddHpSkillClick(GameObject go)
    {
        if (!int.TryParse(go.name.Substring(go.name.Length - 1, 1), out m_Index))
            return;
        if (m_Skill5CD.fillAmount > 0)
            return;
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

        GameData.m_GameManager.m_UIManager.m_UpdateSkillCDUICallback = InitSkillCD;
        GameData.m_GameManager.m_UIManager.m_UpdateAddHpCallback = UpdateAddHp;
    }

    private void OnSelectedHeroClick(GameObject go)
    {
        for (int i = 0; i < m_HeroItemArray.Count; i++)
        {
            HeroItem item = m_HeroItemArray[i].GetComponent<HeroItem>();
            if (m_HeroItemArray[i] != go)
                item.m_SelectedGo.SetActive(false);
            else
            {
                item.m_SelectedGo.SetActive(true);
                GameData.m_HeroId = item.m_HeroId;
            }
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

    private void InitSkillCD(int cdTime, int index)
    {
        Fix64 cd = (Fix64)cdTime;
        UISprite skillCDUISprite = null;
        if (index == 4)
            skillCDUISprite = m_Skill1CD;
        if (index == 5)
            skillCDUISprite = m_Skill2CD;
        if (index == 6)
            skillCDUISprite = m_Skill3CD;
        if (index == 7)
            skillCDUISprite = m_Skill4CD;
        if (index == 8)
            skillCDUISprite = m_Skill5CD;
        Delay delay = new Delay();
        delay.InitSkillCD(skillCDUISprite, cd);
        GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
    }

    /// <summary>
    /// 刷新是否触发加血
    /// </summary>
    /// <param name="player"></param>
    private void UpdateAddHp(Player player)
    {
        GameObject addHpGo = null;
        Fix64 redDistince = Fix64.Zero;
        Fix64 blueDistince = Fix64.Zero;
        if (player == null || player.m_PlayerData == null)
            return;
        int addHp = player.m_PlayerData.m_HeroAttrNode.hp - player.m_PlayerData.m_HP;
        if (addHp <= 0)
            return;
        if (m_RedPos != null && player.m_PlayerData.m_CampId == 2 && FixVector3.Distance(player.m_Pos, (FixVector3)m_RedPos.transform.position) <= Fix64.FromRaw(1000))
            player.AddHp(addHp);
        if (m_BluePos != null && player.m_PlayerData.m_CampId == 1 && FixVector3.Distance(player.m_Pos, (FixVector3)m_BluePos.transform.position) <= Fix64.FromRaw(1000))
            player.AddHp(addHp);
        if (m_RedAddHp != null && m_RedAddHp.activeSelf && FixVector3.Distance(player.m_Pos, (FixVector3)m_RedAddHp.transform.position) <= Fix64.FromRaw(200))
            addHpGo = m_RedAddHp;
        if (m_BlueAddHp != null && m_BlueAddHp.activeSelf && FixVector3.Distance(player.m_Pos, (FixVector3)m_BlueAddHp.transform.position) <= Fix64.FromRaw(200))
            addHpGo = m_BlueAddHp;
        if (addHpGo == null)
            return;
        player.AddHp(300);
        addHpGo.SetActive(false);
        Delay delay = new Delay();
        delay.InitAddHP(addHpGo, Fix64.FromRaw(10000));
        GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
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
