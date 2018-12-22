using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后仰状态
/// </summary>
public class HitState : BaseState
{
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //动画状态机
    private Animator m_Animator;
    //动画名称
    private string m_StateParameter = "State";
#endif
    #endregion
    private Fix64 m_AniTime = Fix64.FromRaw(500);
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
        m_BaseObject.m_IsHit = true;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 13);
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
        if (!m_BaseObject.m_IsHit)
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
        m_BaseObject.m_IsHit = false;
        m_BaseObject.m_IsSkill = false;
        m_BaseObject.m_IsAttack = false;
        m_BaseObject.m_IsMove = false;
        m_IntervalTime = Fix64.Zero;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
    }
}
