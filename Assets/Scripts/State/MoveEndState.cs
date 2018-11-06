using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动结束状态
/// </summary>
public class MoveEndState : BaseState
{
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //动画状态机
    private Animator m_Animator;
    //动画名称
    private string m_StateParameter = "State";
#endif
    #endregion
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(Player player, string parameter = null)
    {
        base.OnInit(player, parameter);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = m_Player.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        m_Player.m_IsMove = false;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
        //if (GameData.m_GameManager != null && GameData.m_GameManager.m_LogMessage != null)
        //    GameData.m_GameManager.m_LogMessage.text += string.Format("{0}:{1},", GameData.m_GameFrame, m_Player.m_VGo.transform.position);
        //if (GameData.m_GameManager != null && GameData.m_GameManager.m_LogMessage != null)
        //    GameData.m_GameManager.m_LogMessage.text += string.Format("{0}:{1},", GameData.m_GameFrame, m_Player.m_Pos);
    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }
}
