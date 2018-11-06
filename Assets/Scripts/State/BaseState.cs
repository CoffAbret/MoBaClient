using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机基类
/// </summary>
public class BaseState
{
    //游戏对象
    protected Player m_Player;
    //参数
    protected string m_Parameter;

    /// <summary>
    /// 初始化状态数据
    /// </summary>
    /// <param name="player">游戏对象</param>
    /// <param name="parameter">参数</param>
    public virtual void OnInit(Player player, string parameter = null)
    {
        m_Player = player;
        m_Parameter = parameter;
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    /// <param name="aniName"></param>
    public virtual void OnEnter()
    {

    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    /// <param name="parameter"></param>
    public virtual void UpdateLogic()
    {

    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public virtual void OnExit()
    {
        m_Parameter = null;
    }
}
