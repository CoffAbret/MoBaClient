using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据类
/// </summary>
public class GameData
{
    //游戏管理器
    public static GameManager m_GameManager;
    //当前角色id
    public static int m_CurrentRoleId;
    //当前选择英雄id
    public static int m_HeroId = 201991000;
    //当前阵营
    public static int m_CampId;
    //当前显示角色
    public static Player m_CurrentPlayer;
    //每帧时间长度
    public static Fix64 m_FixFrameLen = Fix64.FromRaw(20);
    //随机数
    public static SRandom m_Srandom = new SRandom(1000);
    //游戏逻辑帧数
    public static int m_GameFrame;
    //Ping累计时长
    public static Fix64 m_PingTime = Fix64.Zero;
    //所有显示角色列表
    public static List<Player> m_PlayerList = new List<Player>();
    //所有显示箭塔列表
    public static List<Tower> m_TowerList = new List<Tower>();
    //所有操作事件的列表
    public static List<FrameKeyData> m_OperationEventList = new List<FrameKeyData>();
    //技能特效路径
    public static string m_EffectPath = "Effect/Prefabs";
    //是否执行显示层逻辑
    public static bool m_IsExecuteViewLogic = true;
    //游戏是否开始
    public static bool m_IsGame = false;
    //客户端帧数
    public static int m_ClientGameFrame;
    //战斗结果
    public static bool m_GameResult = false;
    //死亡次数
    public static int m_DieCount = 0;
    //是否正在查看小地图
    public static bool m_IsDragMinMap;
    //IP
    public static string m_IP = "192.168.3.55";
    //端口
    public static int m_Port = 8889;
    //UDPIP
    public static string m_UdpIP = "192.168.3.55";
    //UDP端口
    public static int m_UdpPort = 8888;
    //普攻按钮点击次数
    public static int m_AttackClickIndex = 0;
}
