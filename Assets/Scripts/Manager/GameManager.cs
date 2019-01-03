using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager
{
    //网络连接管理
    public NetManager m_NetManager;
    //移动管理器
    public PlayerMoveManager m_PlayerMoveManager;
    //操作管理
    public OpreationManager m_OpreationManager;
    //战斗管理
    public BattleLogicManager m_BattleLogicManager;
    //技能子弹管理器
    public BulletManager m_BulletManager;
    //延时管理
    public DelayManager m_DelayManager;
    //游戏物体生成管理器
    public SpawnManager m_SpawnManager;
    //UI管理器
    public UIManager m_UIManager;
    //网格地图管理器
    public GridManager m_GridManager;
    //Log输出
    public UILabel m_LogMessage;

    /// <summary>
    /// 初始化TCP网络
    /// </summary>
    public void InitTcpNet()
    {
        m_NetManager = new NetManager();
        m_NetManager.InitTcpClient();
    }

    /// <summary>
    /// 初始化UDP网络
    /// </summary>
    public void InitUdpNet()
    {
        if (m_NetManager == null)
            return;
        m_NetManager.InitUdpClient();
    }

    /// <summary>
    /// 初始化使用Tcp网络的游戏数据
    /// </summary>
    public void InitTcpGame()
    {
        m_OpreationManager = new OpreationManager();
        m_UIManager = new UIManager();
        m_DelayManager = new DelayManager();
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_LogMessage = GameObject.Find("LogMessage").GetComponent<UILabel>();
        #endregion
    }

    /// <summary>
    /// 初始化使用Udp网络的游戏数据
    /// </summary>
    public void InitUdpGame()
    {
        LoadJsonData();
        m_PlayerMoveManager = new PlayerMoveManager();
        m_BattleLogicManager = new BattleLogicManager();
        m_BulletManager = new BulletManager();
        m_SpawnManager = new SpawnManager();
        m_GridManager = new GridManager();
        m_GridManager.InitGrid();
    }

    /// <summary>
    /// 每帧处理Tcp网络逻辑
    /// </summary>
    public void UpdateTcpNet()
    {
        if (m_NetManager != null)
            m_NetManager.UpdateTcpNet();
    }

    /// <summary>
    /// 每帧处理Udp网络逻辑
    /// </summary>
    public void UpdateUdpNet()
    {
        if (m_NetManager != null)
            m_NetManager.UpdateUdpNet();
    }

    /// <summary>
    /// 每帧处理使用Tcp网络的游戏逻辑
    /// </summary>
    public void UpdateTcpGame()
    {
        if (GameData.m_GameManager.m_DelayManager != null)
            GameData.m_GameManager.m_DelayManager.UpdateDelay();
        if (GameData.m_GameManager.m_PlayerMoveManager != null)
            GameData.m_GameManager.m_PlayerMoveManager.UpdateMove();
    }

    /// <summary>
    /// 每帧处理使用UDP网络的游戏逻辑
    /// </summary>
    public void UpdateUdpGame()
    {
        if (!GameData.m_IsGame)
            return;
        if (GameData.m_GameManager.m_BattleLogicManager != null)
            GameData.m_GameManager.m_BattleLogicManager.UpdateLogic();
        if (GameData.m_GameManager.m_BulletManager != null)
            GameData.m_GameManager.m_BulletManager.UpdateAttack();
        if (GameData.m_GameManager.m_SpawnManager != null)
            GameData.m_GameManager.m_SpawnManager.UpdateLogic();
    }


    /// <summary>
    /// 销毁游戏数据
    /// </summary>
    public void DestoryGame()
    {
        for (int i = 0; i < GameData.m_ObjectList.Count; i++)
            GameData.m_ObjectList[i].Destroy();
        GameData.m_ObjectList.Clear();
        if (m_NetManager != null)
            m_NetManager.OnDisconnect();
        if (m_DelayManager != null)
            m_DelayManager.DestoryDelay();
        if (m_BulletManager != null)
            m_BulletManager.DestoryAttack();
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="campId">失败阵营</param>
    public void GameOver(CampType campId)
    {
        GameData.m_IsGame = false;
        GameData.m_GameResult = GameData.m_CampId == campId ? false : true;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            GameData.m_GameManager.m_UIManager.m_SettlementUICallback(GameData.m_GameResult);
        }
        #endregion
    }
    /// <summary>
    /// 准备操作
    /// </summary>
    public void InputReady()
    {
        CWritePacket packet = m_OpreationManager.InputReady();
        m_NetManager.SendUdp(packet);
    }

    /// <summary>
    /// 输入操作
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public void InputCmd(Cmd cmd, string parameter = null)
    {
        CWritePacket packet = m_OpreationManager.InputCmd(cmd, parameter);
        m_NetManager.SendUdp(packet);
        //LogMsg(string.Format("发送操作:{0}，参数：{1},帧数{2}", cmd.ToString(), parameter, GameData.m_GameFrame));
    }

    /// <summary>
    /// 登录游戏
    /// </summary>
    /// <param name="account"></param>
    public void InputLogin(string account)
    {
        CWritePacket packet = m_OpreationManager.InputLogin(account);
        m_NetManager.Send(packet);
    }

    /// <summary>
    /// 参加匹配
    /// </summary>
    /// <param name="account"></param>
    public void InputMatch(int matchType)
    {
        CWritePacket packet = m_OpreationManager.InputMatch(matchType);
        m_NetManager.Send(packet);
    }

    /// <summary>
    /// 进入匹配房间
    /// </summary>
    /// <param name="account"></param>
    public void InputJoinMatchRoom()
    {
        CWritePacket packet = m_OpreationManager.InputJoinMatchRoom();
        m_NetManager.Send(packet);
    }
    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="charData"></param>
    public void CreatePlayer(PlayerData playerData)
    {
        Player plyerObj = new Player();
        plyerObj.Create(playerData);
        GameData.m_ObjectList.Add(plyerObj);
    }

    /// <summary>
    /// 创建箭塔
    /// </summary>
    /// <param name="charData"></param>
    public void CreateTower(CampType campId, ObjectType type)
    {
        Tower towerObj = new Tower();
        int hp = type == ObjectType.CRYSTAL_TOWER ? 20000 : 10000;
        TowerData data = new TowerData(campId, type, hp);
        towerObj.Create(data);
        GameData.m_ObjectList.Add(towerObj);
    }

    /// <summary>
    /// 创建所有角色
    /// </summary>
    /// <param name="playerStr"></param>
    public void CreateAllPlayer(object[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Dictionary<string, object> playerDic = data[i] as Dictionary<string, object>;
            int roleId = playerDic.TryGetInt("playerId");
            string roleName = playerDic.TryGetString("playerName");
            int heroId = int.Parse(playerDic.TryGetString("heroId"));
            int campId = int.Parse(playerDic.TryGetString("teamId"));
            campId = campId == 0 ? 1 : 2;
            PlayerData charData = new PlayerData(roleId, heroId, roleName, (CampType)campId, ObjectType.PLAYER);
            CreatePlayer(charData);
        }
        for (int i = 0; i < 2; i++)
        {
            int campId = i % 2 == 0 ? 1 : 2;
            CreateTower((CampType)campId, ObjectType.ARROW_TOWER);
        }
        for (int i = 0; i < 2; i++)
        {
            int campId = i % 2 == 0 ? 1 : 2;
            CreateTower((CampType)campId, ObjectType.CRYSTAL_TOWER);
        }
        m_GridManager.InitTowerGrid();
        InitLog();
    }

    public void SyncKey(Dictionary<string, object> data)
    {
        int serverFrameCount = 0;
        serverFrameCount = data.TryGetInt("framecount");
        GameData.m_GameFrame = serverFrameCount;
        //补帧逻辑，后续添加
        //if (serverFrameCount >= GameData.m_GameFrame)
        //{
        FrameKeyData frameKeyData = new FrameKeyData();
        frameKeyData.m_KeyDataList = new List<KeyData>();
        frameKeyData.m_FrameCount = serverFrameCount;
        GameData.m_OperationEventList.Add(frameKeyData);
        if (data["keydatalist"] == null)
            return;
        object[] keydataArray = data["keydatalist"] as object[];
        for (int i = 0; i < keydataArray.Length; i++)
        {
            Dictionary<string, object> keydata = keydataArray[i] as Dictionary<string, object>;
            KeyData m_KeyData = new KeyData();
            m_KeyData.m_RoleId = keydata.TryGetInt("m_RoleId");
            m_KeyData.m_Cmd = keydata.TryGetInt("m_Cmd");
            m_KeyData.m_Parameter = keydata.TryGetString("m_Parameter");
            frameKeyData.m_KeyDataList.Add(m_KeyData);
            m_BattleLogicManager.OnOperation(m_KeyData);
            LogMsg(string.Format("收到操作:{0}，参数：{1},帧数：{2}", m_KeyData.m_Cmd.ToString(), m_KeyData.m_Parameter, GameData.m_GameFrame));
        }
        //GameData.m_GameFrame += 1;
        //}
    }
    /// <summary>
    /// 登录游戏
    /// </summary>
    /// <param name="data"></param>
    public void LoginGame(Dictionary<string, object> data)
    {
        int result = data.TryGetInt("ret");
        int playerId = data.TryGetInt("playerId");
        GameData.m_CurrentRoleId = playerId;
        if (result != 0)
            return;
        m_UIManager.m_UpdateMatchUICallback();
    }

    /// <summary>
    /// 匹配游戏
    /// </summary>
    /// <param name="data"></param>
    public void MatchGame(Dictionary<string, object> data)
    {
        int result = data.TryGetInt("ret");
        if (result != 0)
            return;
        Delay delay = new Delay();
        delay.InitMatchTime(Fix64.FromRaw(30000), UpdateMatchTime);
        m_DelayManager.m_DelayList.Add(delay);
    }

    /// <summary>
    /// 匹配游戏成功
    /// </summary>
    /// <param name="data"></param>
    public void MatchGameSuccess(Dictionary<string, object> data)
    {
        int result = data.TryGetInt("ret");
        if (result != 0)
            return;
        string matchKey = data.TryGetString("matchKey");
        GameData.m_CampId = (CampType)data.TryGetInt("teamId");
        GameData.m_MatchPos = data.TryGetInt("pos");
        GameData.m_MatchKey = matchKey;
        if (data["teamInfo"] == null)
            return;
        object[] teamInfoArray = data["teamInfo"] as object[];
        List<MatchPlayerData> matchPlayerList = new List<MatchPlayerData>();
        for (int i = 0; i < teamInfoArray.Length; i++)
        {
            Dictionary<string, object> keydata = teamInfoArray[i] as Dictionary<string, object>;
            MatchPlayerData matchPlayer = new MatchPlayerData();
            matchPlayer.m_CampId = keydata.TryGetInt("teamId");
            matchPlayer.m_Pos = keydata.TryGetInt("pos");
            matchPlayerList.Add(matchPlayer);
        }
        m_UIManager.m_UpdateMatchSuccessUICallback();
    }

    /// <summary>
    /// 确认进入匹配房间
    /// </summary>
    /// <param name="data"></param>
    public void JoinMatchRoom(Dictionary<string, object> data)
    {
        int result = data.TryGetInt("ret");
        if (result != 0)
            return;
    }

    /// <summary>
    /// 点亮匹配头像
    /// </summary>
    /// <param name="data"></param>
    public void JoinMatchRoomInPos(Dictionary<string, object> data)
    {
        int campId = data.TryGetInt("teamId");
        int pos = data.TryGetInt("pos");
        GameData.m_GameManager.m_UIManager.m_UpdateSelectHeroConfirmMatchUI(campId, pos);
    }

    /// <summary>
    /// 进入匹配房间
    /// </summary>
    /// <param name="data"></param>
    public void JoinMatchHeroRoom(Dictionary<string, object> data)
    {
        int result = data.TryGetInt("ret");
        if (result != 0)
            return;
        string mobaKey = data.TryGetString("mobaKey");
        string udpIp = data.TryGetString("ip");
        int udpPort = data.TryGetInt("port");
        GameData.m_UdpIP = udpIp;
        GameData.m_UdpPort = udpPort;
        GameData.m_MobaKey = mobaKey;
        m_UIManager.m_UpdateSelectHeroUI();
    }

    /// <summary>
    /// 匹配游戏倒计时
    /// </summary>
    /// <param name="time"></param>
    private void UpdateMatchTime(int time)
    {
        m_UIManager.m_UpdateMatchCountdownUICallback(time);
    }

    public void InitLog()
    {
        //        GameData.m_logFilePath = string.Format("{0}/{1}_Log.txt", Application.dataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        //#if UNITY_IOS || UNITY_ANDROID
        //                GameData.m_logFilePath = string.Format("{0}/{1}_Log.txt", Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        //#endif
        //        System.IO.FileStream fs = new System.IO.FileStream(GameData.m_logFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        //        fs.Dispose();
        //        fs.Close();
    }

    public void LogMsg(string log)
    {
        //System.IO.FileStream fs = new System.IO.FileStream(GameData.m_logFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
        //System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
        //sw.WriteLine(log);
        //sw.Dispose();
        //sw.Close();
        //fs.Dispose();
        //fs.Close();
    }
    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public void LoadJsonData()
    {
        FSDataNodeTable<SkillNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("skill"));                 //技能表
        FSDataNodeTable<BulletNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("bullet"));               //子弹表
        FSDataNodeTable<ModelNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("model"));                 //模型表
        FSDataNodeTable<HeroNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("hero"));                   //英雄表
        FSDataNodeTable<HeroSkinNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("skin"));               //英雄皮肤表
        FSDataNodeTable<HeroAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("heroAttr"));           //英雄属性表

        FSDataNodeTable<MonsterAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("monsterAttr"));     //怪物属性表
        FSDataNodeTable<MonsterSkillNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("skill_monster"));  //怪物技能表

        FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("Moba3v3NaviNode")); //地图寻路表
    }
}
