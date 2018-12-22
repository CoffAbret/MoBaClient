using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡状态
/// </summary>
public class DieState : BaseState
{
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //动画状态机
    private Animator m_Animator;
    //动画名称
    private string m_StateParameter = "State";
#endif
    #endregion
    private Fix64 m_AniTime = Fix64.FromRaw(800);
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(BaseObject baseObject, string parameter = null)
    {
        base.OnInit(baseObject, parameter);
        if (baseObject == null)
            return;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = baseObject.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (m_BaseObject == null)
            return;
        m_BaseObject.m_IsDie = true;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 12);
        #endregion
    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (m_BaseObject == null)
            return;
        if (!m_BaseObject.m_IsDie)
            return;
        m_IntervalTime += GameData.m_FixFrameLen;
        if (m_IntervalTime >= m_AniTime)
            OnExit();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_BaseObject == null)
            return;
        if (m_BaseObject.m_Data.m_Type == ObjectType.PLAYER)
        {
            GameData.m_DieCount++;
            Fix64 resurgenceTime = Fix64.FromRaw(20000) * (Fix64)GameData.m_DieCount;
            Delay delay = new Delay();
            delay.InitResurgence((m_BaseObject as Player).m_PlayerData, resurgenceTime, GameData.m_GameManager.CreatePlayer);
            GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
        }
        m_IntervalTime = Fix64.Zero;
        m_BaseObject.Destroy();
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath(string game)
    {
        if (Application.isMobilePlatform)
        {
            return Application.persistentDataPath + "/" + game + "/";
        }
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return Application.streamingAssetsPath + "/";
        }
        if (Application.isEditor)
        {
            return Application.streamingAssetsPath + "/";
        }
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            int i = Application.dataPath.LastIndexOf('/');
            return Application.dataPath.Substring(0, i + 1) + game + "/";
        }
        return "c:/" + game + "/";
    }
}