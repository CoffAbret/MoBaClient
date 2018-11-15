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
    private Fix64 m_AniTime = Fix64.FromRaw(4000);
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(Player viewPlayer, string parameter = null)
    {
        base.OnInit(viewPlayer, parameter);
        if (m_Player == null)
            return;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = viewPlayer.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (m_Player == null)
            return;
        m_Player.m_IsDie = true;
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
        if (m_Player == null)
            return;
        if (!m_Player.m_IsDie)
            return;
        m_Player.m_IntervalTime += GameData.m_FixFrameLen;
        if (m_Player.m_IntervalTime >= m_AniTime)
            OnExit();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_Player == null)
            return;
        if (m_Player.m_PlayerData.m_Type == 1)
        {
            GameData.m_DieCount++;
            Fix64 resurgenceTime = Fix64.FromRaw(20000) * GameData.m_DieCount;
            Delay delay = new Delay();
            delay.InitResurgence(m_Player.m_PlayerData, resurgenceTime, GameData.m_GameManager.CreatePlayer);
            GameData.m_GameManager.m_DelayManager.m_DelayList.Add(delay);
        }
        m_Player.m_IntervalTime = Fix64.Zero;
        m_Player.Destroy();
    }
}
