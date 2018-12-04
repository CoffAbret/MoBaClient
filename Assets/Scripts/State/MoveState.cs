using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    Fix64 m_MoveSpeed;
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
    public override void OnInit(Player viewPlayer, string parameter = null)
    {
        base.OnInit(viewPlayer, parameter);
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        if (m_Parameter == null || !m_Parameter.Contains("#"))
            return;
        float x = float.Parse(m_Parameter.Split('#')[0]);
        float z = float.Parse(m_Parameter.Split('#')[2]);
        m_Player.m_Angles = (FixVector3)(new Vector3(x, 0, z));
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
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        m_Player.m_IsMove = true;
        Quaternion targetRotation = Quaternion.LookRotation((m_Player.m_Pos + m_Player.m_Angles - m_Player.m_Pos).ToVector3(), Vector3.up);
        m_Player.m_Rotation = (FixVector3)(targetRotation.eulerAngles);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            //插值旋转可以优化旋转抖动，不流畅等问题
            //m_Player.m_VGo.transform.rotation = Quaternion.Slerp(m_Player.m_VGo.transform.rotation, targetRotation, (float)(GameData.m_FixFrameLen * (Fix64)10));
            m_Player.m_VGo.transform.rotation = targetRotation;
            m_Animator.SetInteger(m_StateParameter, 11);
        }
        #endregion
    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        if (!m_Player.m_IsMove)
            return;
        FixVector3 fixPos = m_Player.m_Pos + ((Fix64)m_Player.m_PlayerData.m_HeroAttrNode.movement_speed * m_Player.m_Angles * GameData.m_FixFrameLen);
        Vector2 gridPos = GameData.m_GameManager.m_GridManager.MapPosToGrid(fixPos.ToVector3());
        bool isWalk = GameData.m_GameManager.m_GridManager.GetWalkable(gridPos);
        if (isWalk)
        {
            m_Player.m_IntervalTime += GameData.m_FixFrameLen;
            m_Player.m_Pos = fixPos;
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
                m_Player.m_VGo.transform.position = m_Player.m_Pos.ToVector3();
            #endregion
        }
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        m_Player.m_IsMove = false;
        m_Player.m_IntervalTime = Fix64.Zero;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
    }
}
