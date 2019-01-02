using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class MainBehaviour : MonoBehaviour
{
    //登录UI
    public GameObject m_LoginGo;
    public GameObject m_LoginButton;
    public GameObject m_LoginConnectTcpNetButton;
    public GameObject m_LoginExitButton;
    public UILabel m_LoginAccount;
    public UILabel m_LoginIPTxt;
    public UILabel m_LoginPortTxt;

    //匹配UI
    public GameObject m_MatchGo;
    public GameObject m_Match1V1Button;
    public GameObject m_Match3V3Button;
    public UILabel m_MatchCountdown;

    //匹配成功UI
    public GameObject m_MatchSuccessGo;
    public GameObject m_MatchSuccessJoinMatchButton;

    //选择英雄UI
    public GameObject m_SelectHeroGo;
    public List<GameObject> m_SelectHeroItemList;
    public GameObject m_SelectHeroJoinGameButton;
    public GameObject m_SelectHeroExitGameButton;

    //战斗界面UI
    public GameObject m_BattleGo;
    public GameObject m_BattleAttackButton;
    public GameObject m_BattleSkill1Button;
    public UISprite m_BattleSkill1CD;
    public GameObject m_BattleSkill2Button;
    public UISprite m_BattleSkill2CD;
    public GameObject m_BattleSkill3Button;
    public UISprite m_BattleSkill3CD;
    public GameObject m_BattleSkill4Button;
    public UISprite m_BattleSkill4CD;
    public GameObject m_BattleSkill5Button;
    public UISprite m_BattleSkill5CD;

    //小地图UI
    public GameObject m_MiniMapGo;

    //复活界面UI
    public GameObject m_ResurrectionCountdownGo;
    public UILabel m_ResurrectionCountdown;

    //敌方复活UI
    public GameObject m_EmenyResurrectionCountdownGo;
    public UISprite m_EmenyResurrectionCountdownHeroHead;
    public UILabel m_EmenyResurrectionCountdown;

    //游戏结束UI
    public GameObject m_SettlementGo;
    public GameObject m_SettlementShengLi;
    public GameObject m_SettlementShiBai;
    public GameObject m_SettlemengExitGameButton;

    //主摄像机
    public GameObject m_MainCamera;

    //登录摄像机
    public GameObject m_LoginCamera;

    //登录场景
    public GameObject m_LoginScene;

    //MoBa场景
    public GameObject m_MoBa1v1Scene;
    public GameObject m_MoBa1v1SceneRedAddHpPos;
    public GameObject m_MoBa1v1SceneBlueAddHpPos;
    public GameObject m_MoBa1v1SceneRedResurrectionPos;
    public GameObject m_MoBa1v1SceneBlueResurrectionPos;
    //Update累计时间
    private Fix64 m_UpdateCumulativeTime = Fix64.Zero;
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        //允许后台运行
        Application.runInBackground = true;
        //固定50帧
        Application.targetFrameRate = 50;
        //初始化游戏管理器
        GameData.m_GameManager = new GameManager();
        //创建日志文件
        GameData.m_GameManager.InitLog();
    }
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void Start()
    {
        UIEventListener.Get(m_LoginButton).onClick = OnLoginClick;
        UIEventListener.Get(m_LoginConnectTcpNetButton).onClick = OnLoginConnectTcpNetClick;
        UIEventListener.Get(m_LoginExitButton).onClick = OnExitGameClick;

        UIEventListener.Get(m_Match1V1Button).onClick = OnMatch1v1Click;
        UIEventListener.Get(m_MatchSuccessJoinMatchButton).onClick = OnMatchSuccessJoinMatchClick;

        UIEventListener.Get(m_SelectHeroExitGameButton).onClick = OnExitGameClick;
        UIEventListener.Get(m_SelectHeroJoinGameButton).onClick = OnJoinGameClick;
        for (int i = 0; i < m_SelectHeroItemList.Count; i++)
            UIEventListener.Get(m_SelectHeroItemList[i]).onClick = OnSelectedHeroItemClick;

        UIEventListener.Get(m_BattleAttackButton).onClick = OnBattleAttackClick;
        UIEventListener.Get(m_BattleSkill1Button).onClick = OnBattleSkillClick;
        UIEventListener.Get(m_BattleSkill2Button).onClick = OnBattleSkillClick;
        UIEventListener.Get(m_BattleSkill3Button).onClick = OnBattleSkillClick;
        UIEventListener.Get(m_BattleSkill4Button).onClick = OnBattleSkillClick;
        UIEventListener.Get(m_BattleSkill5Button).onClick = OnBattleSkillClick;

        UIEventListener.Get(m_SettlemengExitGameButton).onClick = OnExitGameClick;
    }

    /// <summary>
    /// 每帧接收网络数据
    /// </summary>
    private void FixedUpdate()
    {
        if (GameData.m_GameManager == null)
            return;
        if (GameData.m_GameManager.m_NetManager == null)
            return;
        //保证逻辑更新频率固定
        m_UpdateCumulativeTime += (Fix64)Time.deltaTime;
        if (m_UpdateCumulativeTime >= GameData.m_FixFrameLen)
        {
            GameData.m_GameManager.UpdateTcpNet();
            GameData.m_GameManager.UpdateUdpNet();
            GameData.m_GameManager.UpdateTcpGame();
            GameData.m_GameManager.WriteLog();
            m_UpdateCumulativeTime = Fix64.Zero;
        }
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
    /// 连接Tcp网络
    /// </summary>
    /// <param name="go"></param>
    private void OnLoginConnectTcpNetClick(GameObject go)
    {
        int port;
        GameData.m_IP = m_LoginIPTxt.text.Trim();
        int.TryParse(m_LoginPortTxt.text.Trim(), out port);
        GameData.m_Port = port;
        GameData.m_GameManager.InitTcpNet();
        GameData.m_GameManager.InitTcpGame();
        OnInitUICallback();
    }

    /// <summary>
    /// 登录游戏
    /// </summary>
    /// <param name="go"></param>
    private void OnLoginClick(GameObject go)
    {
        string account = m_LoginAccount.text;
        GameData.m_GameManager.InputLogin(account);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    /// <param name="go"></param>
    private void OnExitGameClick(GameObject go)
    {
        Application.Quit();
    }

    /// <summary>
    /// 1v1匹配
    /// </summary>
    /// <param name="go"></param>
    public void OnMatch1v1Click(GameObject go)
    {
        GameData.m_GameManager.InputMatch(1);
        m_MatchCountdown.gameObject.SetActive(true);
        m_MatchCountdown.text = string.Empty;
    }

    /// <summary>
    /// 进入匹配房间
    /// </summary>
    /// <param name="go"></param>
    public void OnMatchSuccessJoinMatchClick(GameObject go)
    {
        GameData.m_GameManager.InputJoinMatchRoom();
    }

    /// <summary>
    /// 选择英雄
    /// </summary>
    /// <param name="go"></param>
    private void OnSelectedHeroItemClick(GameObject go)
    {
        for (int i = 0; i < m_SelectHeroItemList.Count; i++)
        {
            HeroItem item = m_SelectHeroItemList[i].GetComponent<HeroItem>();
            if (m_SelectHeroItemList[i] != go)
                item.m_SelectedGo.SetActive(false);
            else
            {
                item.m_SelectedGo.SetActive(true);
                GameData.m_HeroId = item.m_HeroId;
            }
        }
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    /// <param name="go"></param>
    private void OnJoinGameClick(GameObject go)
    {
        GameData.m_GameManager.InitUdpNet();
        GameData.m_GameManager.InputReady();
        GameData.m_GameManager.InitUdpGame();
    }

    //普攻切换时间
    private Fix64 m_AttackTime = Fix64.FromRaw(800);
    /// <summary>
    /// 点击普攻
    /// </summary>
    /// <param name="go"></param>
    private void OnBattleAttackClick(GameObject go)
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
    private void OnBattleSkillClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        if (GameData.m_CurrentPlayer.m_IsDie)
            return;
        int m_Index;
        if (!int.TryParse(go.name.Substring(go.name.Length - 1, 1), out m_Index))
            return;
        if (m_Index == 4 && m_BattleSkill1CD.fillAmount > 0)
            return;
        if (m_Index == 5 && m_BattleSkill2CD.fillAmount > 0)
            return;
        if (m_Index == 6 && m_BattleSkill3CD.fillAmount > 0)
            return;
        if (m_Index == 7 && m_BattleSkill4CD.fillAmount > 0)
            return;
        if (m_Index == 8 && m_BattleSkill5CD.fillAmount > 0)
            return;
        GameData.m_CurrentPlayer.m_IsSkill = true;
        GameData.m_GameManager.InputCmd(Cmd.UseSkill, m_Index.ToString() + "#5#0#5");
    }


    /// <summary>
    /// 初始化UI回调
    /// </summary>
    private void OnInitUICallback()
    {
        GameData.m_GameManager.m_UIManager.m_UpdateMatchUICallback = OnUpdateMatchUI;
        GameData.m_GameManager.m_UIManager.m_UpdateMatchCountdownUICallback = OnUpdateMatchCountdownUI;
        GameData.m_GameManager.m_UIManager.m_UpdateMatchSuccessUICallback = OnUpdateMatchSuccessUI;

        GameData.m_GameManager.m_UIManager.m_UpdateSelectHeroUI = OnUpdateSelectHeroUI;
        GameData.m_GameManager.m_UIManager.m_UpdateSelectHeroConfirmMatchUI = OnUpdateSelectHeroConfirmMatchUI;

        GameData.m_GameManager.m_UIManager.m_UpdateBattleUICallback = OnUpdateBattleUI;
        GameData.m_GameManager.m_UIManager.m_UpdateBattleSkillUICallback = OnUpdateBattleSkillUI;

        GameData.m_GameManager.m_UIManager.m_UpdateResurrectionCountdownUICallback = OnUpdateResurrectionCountdownUI;
        GameData.m_GameManager.m_UIManager.m_ResurrectionCountdown = m_ResurrectionCountdown;

        GameData.m_GameManager.m_UIManager.m_UpdateEnemyResurrectionCountdownUICallback = OnUpdateEnemyResurrectionCountdownUI;

        GameData.m_GameManager.m_UIManager.m_EmenyResurrectionCountdown = m_EmenyResurrectionCountdown;
        GameData.m_GameManager.m_UIManager.m_SettlementUICallback = OnSettlementUI;

        GameData.m_GameManager.m_UIManager.m_UpdateBattleSkillCDUICallback = OnUpdateBattleSkillCD;
        GameData.m_GameManager.m_UIManager.m_UpdateAddHpCallback = OnUpdateAddHp;
    }

    /// <summary>
    /// 刷新匹配UI回调
    /// </summary>
    private void OnUpdateMatchUI()
    {
        m_LoginGo.SetActive(false);
        m_MatchGo.SetActive(true);
        m_MatchCountdown.gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新匹配倒计时回调
    /// </summary>
    private void OnUpdateMatchCountdownUI(int time)
    {
        m_MatchCountdown.text = string.Format("{0}", time);
    }

    /// <summary>
    /// 匹配成功UI回调
    /// </summary>
    private void OnUpdateMatchSuccessUI()
    {
        m_MatchGo.SetActive(false);
        m_MatchSuccessGo.SetActive(true);
    }

    /// <summary>
    /// 进入选择英雄UI回调
    /// </summary>
    private void OnUpdateSelectHeroUI()
    {
        m_MatchSuccessGo.SetActive(false);
        m_SelectHeroGo.SetActive(true);
    }

    /// <summary>
    /// 选择英雄后确认参加匹配回调
    /// </summary>
    /// <param name="campId"></param>
    /// <param name="pos"></param>
    private void OnUpdateSelectHeroConfirmMatchUI(int campId, int pos)
    {
        Transform campTransform = m_MatchSuccessGo.transform.Find(string.Format("Camp{0}", campId));
        if (campTransform == null)
            return;
        Transform posTransform = campTransform.Find(string.Format("Player{0}", pos));
        if (posTransform == null)
            return;
        posTransform.Find("selected").gameObject.SetActive(true);
    }

    /// <summary>
    /// 进入游戏后战斗UI回调
    /// </summary>
    private void OnUpdateBattleUI()
    {
        m_LoginCamera.SetActive(false);
        m_LoginScene.SetActive(false);
        m_MainCamera.SetActive(true);
        m_MoBa1v1Scene.SetActive(true);
        m_SelectHeroGo.SetActive(false);
        m_MiniMapGo.SetActive(true);
        m_BattleGo.SetActive(true);
    }

    /// <summary>
    /// 刷新当前角色技能UI回调
    /// </summary>
    /// <param name="skillNodeList"></param>
    private void OnUpdateBattleSkillUI(List<SkillNode> skillNodeList)
    {
        m_BattleSkill1Button.GetComponent<UISprite>().spriteName = skillNodeList[3].skill_icon;
        m_BattleSkill2Button.GetComponent<UISprite>().spriteName = skillNodeList[4].skill_icon;
        m_BattleSkill3Button.GetComponent<UISprite>().spriteName = skillNodeList[5].skill_icon;
        m_BattleSkill4Button.GetComponent<UISprite>().spriteName = skillNodeList[6].skill_icon;

        m_BattleSkill1Button.GetComponent<UIButton>().normalSprite = skillNodeList[3].skill_icon;
        m_BattleSkill2Button.GetComponent<UIButton>().normalSprite = skillNodeList[4].skill_icon;
        m_BattleSkill3Button.GetComponent<UIButton>().normalSprite = skillNodeList[5].skill_icon;
        m_BattleSkill4Button.GetComponent<UIButton>().normalSprite = skillNodeList[6].skill_icon;
    }

    /// <summary>
    /// 刷新当前角色死亡UI回调
    /// </summary>
    /// <param name="skillNodeList"></param>
    private void OnUpdateResurrectionCountdownUI(bool isDie)
    {
        if (isDie)
        {
            m_BattleGo.SetActive(false);
            m_ResurrectionCountdownGo.SetActive(true);
            m_ResurrectionCountdown.text = "";
        }
        else
        {
            m_BattleGo.SetActive(true);
            m_ResurrectionCountdownGo.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新敌方死亡UI回调
    /// </summary>
    /// <param name="isDie"></param>
    private void OnUpdateEnemyResurrectionCountdownUI(bool isDie, PlayerData data)
    {
        if (isDie)
        {
            m_EmenyResurrectionCountdownGo.SetActive(true);
            m_EmenyResurrectionCountdown.text = "";
            m_EmenyResurrectionCountdownHeroHead.spriteName = data.m_HeroAttrNode.icon_name;
        }
        else
        {
            m_EmenyResurrectionCountdownGo.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新技能CDUI回调
    /// </summary>
    /// <param name="cdTime"></param>
    /// <param name="index"></param>
    private void OnUpdateBattleSkillCD(int cdTime, int index)
    {
        Fix64 cd = (Fix64)cdTime;
        UISprite skillCDUISprite = null;
        if (index == 4)
            skillCDUISprite = m_BattleSkill1CD;
        if (index == 5)
            skillCDUISprite = m_BattleSkill2CD;
        if (index == 6)
            skillCDUISprite = m_BattleSkill3CD;
        if (index == 7)
            skillCDUISprite = m_BattleSkill4CD;
        if (index == 8)
            skillCDUISprite = m_BattleSkill5CD;
        Delay delay = new Delay();
        delay.InitSkillCD(skillCDUISprite, cd);
        GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
    }

    /// <summary>
    /// 刷新是否触发加血回调
    /// </summary>
    /// <param name="player"></param>
    private void OnUpdateAddHp(Player player)
    {
        GameObject addHpGo = null;
        Fix64 redDistince = Fix64.Zero;
        Fix64 blueDistince = Fix64.Zero;
        if (player == null || player.m_PlayerData == null)
            return;
        int addHp = player.m_PlayerData.m_HeroAttrNode.hp - player.m_PlayerData.m_HP;
        if (addHp <= 0)
            return;
        if (m_MoBa1v1SceneRedResurrectionPos != null && player.m_PlayerData.m_CampId == CampType.RED && FixVector3.Distance(player.m_Pos, (FixVector3)m_MoBa1v1SceneRedResurrectionPos.transform.position) <= Fix64.One)
            player.AddHp(addHp);
        if (m_MoBa1v1SceneBlueResurrectionPos != null && player.m_PlayerData.m_CampId == CampType.BLUE && FixVector3.Distance(player.m_Pos, (FixVector3)m_MoBa1v1SceneBlueResurrectionPos.transform.position) <= Fix64.One)
            player.AddHp(addHp);
        if (m_MoBa1v1SceneRedAddHpPos != null && m_MoBa1v1SceneRedAddHpPos.activeSelf && FixVector3.Distance(player.m_Pos, (FixVector3)m_MoBa1v1SceneRedAddHpPos.transform.position) <= Fix64.FromRaw(200))
            addHpGo = m_MoBa1v1SceneRedAddHpPos;
        if (m_MoBa1v1SceneBlueAddHpPos != null && m_MoBa1v1SceneBlueAddHpPos.activeSelf && FixVector3.Distance(player.m_Pos, (FixVector3)m_MoBa1v1SceneBlueAddHpPos.transform.position) <= Fix64.FromRaw(200))
            addHpGo = m_MoBa1v1SceneBlueAddHpPos;
        if (addHpGo == null)
            return;
        player.AddHp(300);
        addHpGo.SetActive(false);
        Delay delay = new Delay();
        delay.InitAddHP(addHpGo, Fix64.FromRaw(20000));
        GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
    }

    /// <summary>
    /// 游戏结束UI回调
    /// </summary>
    private void OnSettlementUI(bool result)
    {
        m_BattleGo.SetActive(false);
        m_SettlementGo.SetActive(true);
        if (result)
        {
            m_SettlementShengLi.SetActive(true);
            m_SettlementShiBai.SetActive(false);
        }
        else
        {
            m_SettlementShengLi.SetActive(false);
            m_SettlementShiBai.SetActive(true);
        }
    }
}
