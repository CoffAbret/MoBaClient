using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    //网络连接管理
    public NetManager m_NetManager;
    //操作管理
    public OpreationManager m_OpreationManager;
    //战斗管理
    public BattleLogicManager m_BattleLogicManager;
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
    /// 初始化
    /// </summary>
    public void InitGame()
    {
        LoadJsonData();
        m_NetManager = new NetManager();
        m_NetManager.InitClient();
        m_OpreationManager = new OpreationManager();
        m_BattleLogicManager = new BattleLogicManager();
        m_DelayManager = new DelayManager();
        m_SpawnManager = new SpawnManager();
        m_UIManager = new UIManager();
        m_GridManager = new GridManager();
        m_GridManager.InitGrid();
        m_LogMessage = GameObject.Find("LogMessage").GetComponent<UILabel>();
    }

    /// <summary>
    /// 每帧处理游戏逻辑
    /// </summary>
    public void UpdateGame()
    {
        if (m_NetManager != null)
            m_NetManager.UpdateNet();
        if (!GameData.m_IsGame)
            return;
        GameData.m_ClientGameFrame++;
        //GameData.m_GameManager.m_LogMessage.text += string.Format("帧数:{0},", GameData.m_ClientGameFrame);
        if (GameData.m_GameManager.m_BattleLogicManager != null)
            GameData.m_GameManager.m_BattleLogicManager.UpdateLogic();
        if (GameData.m_GameManager.m_DelayManager != null)
            GameData.m_GameManager.m_DelayManager.UpdateDelay();
        if (GameData.m_GameManager.m_SpawnManager != null)
            GameData.m_GameManager.m_SpawnManager.UpdateLogic();
    }

    /// <summary>
    /// 销毁游戏数据
    /// </summary>
    public void DestoryGame()
    {
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
            GameData.m_PlayerList[i].Destroy();
        for (int i = 0; i < GameData.m_TowerList.Count; i++)
            GameData.m_TowerList[i].Destroy();
        GameData.m_PlayerList.Clear();
        GameData.m_TowerList.Clear();
        m_NetManager.OnDisconnect();
        m_DelayManager.DestoryDelay();
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="campId">失败阵营</param>
    public void GameOver(int campId)
    {
        GameData.m_IsGame = false;
        GameData.m_GameResult = GameData.m_CurrentPlayer.m_PlayerData.m_CampId == campId ? false : true;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject.Find("UIMainPanel").SetActive(false);
            GameObject.Find("UI Root").transform.Find("UITheBattlePanel").gameObject.SetActive(true);
        }
        #endregion
    }
    /// <summary>
    /// 准备操作
    /// </summary>
    public void InputReady()
    {
        CWritePacket packet = m_OpreationManager.InputReady();
        m_NetManager.Send(packet);
    }

    /// <summary>
    /// 输入操作
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public void InputCmd(Cmd cmd, string parameter = null)
    {
        CWritePacket packet = m_OpreationManager.InputCmd(cmd, parameter);
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
        GameData.m_PlayerList.Add(plyerObj);
    }

    /// <summary>
    /// 创建箭塔
    /// </summary>
    /// <param name="charData"></param>
    public void CreateTower(int campId, int type)
    {
        Tower towerObj = new Tower();
        int hp = type == 2 ? 20000 : 10000;
        towerObj.Create(campId, hp, type);
        GameData.m_TowerList.Add(towerObj);
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
            int roleId = playerDic.TryGetInt("id");
            string roleName = playerDic.TryGetString("name");
            int heroId = i % 2 == 0 ? 201001000 : 201003300;
            int campId = i % 2 == 0 ? 1 : 2;
            PlayerData charData = new PlayerData(roleId, heroId, roleName, campId, 1);
            CreatePlayer(charData);
        }
        for (int i = 0; i < 2; i++)
        {
            int campId = i % 2 == 0 ? 1 : 2;
            CreateTower(campId, 1);
        }
        for (int i = 0; i < 2; i++)
        {
            int campId = i % 2 == 0 ? 1 : 2;
            CreateTower(campId, 2);
        }
    }

    public void SyncKey(Dictionary<string, object> data)
    {
        int serverFrameCount = 0;
        //serverFrameCount = data.TryGetInt("framecount");
        GameData.m_GameFrame = data.TryGetInt("framecount");
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
        }
        //GameData.m_GameFrame += 1;
        //}
    }

    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public void LoadJsonData()
    {
        FSDataNodeTable<SkillNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("skill_hero"));            //技能表
        FSDataNodeTable<ModelNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("model"));                 //模型表
        FSDataNodeTable<HeroNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("hero"));                   //英雄表
        FSDataNodeTable<HeroSkinNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("skin"));               //英雄皮肤表
        FSDataNodeTable<HeroAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("heroAttr"));           //英雄属性表

        FSDataNodeTable<MonsterAttrNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("monsterAttr"));     //怪物属性表
        FSDataNodeTable<MonsterSkillNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("skill_monster"));  //怪物技能表

        FSDataNodeTable<Moba3v3NaviNode>.GetSingleton().LoadJson(ResLoad.LoadJsonRes("Moba3v3NaviNode")); //地图寻路表
    }
}
