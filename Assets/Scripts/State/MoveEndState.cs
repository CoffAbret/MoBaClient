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
    public override void OnInit(BaseObject baseObject, string parameter = null)
    {
        base.OnInit(baseObject, parameter);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_BaseObject == null || m_BaseObject.m_VGo == null)
                return;
            if (m_Animator == null)
                m_Animator = m_BaseObject.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        m_BaseObject.m_IsMove = false;
        m_IntervalTime = Fix64.Zero;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            if (m_Animator != null)
                m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
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
