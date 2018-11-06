using UnityEngine;
using System.Collections;

public enum Status
{
    Prepare,
    Attack1,
    Attack2,
    Attack3,
    Skill1,
    Skill2,
    Skill3,
    Skill4
}

public enum AnimatorParamType
{
    SetInteger,
    SetFloat,
    SetBool,
    SetTrigger,
}

public enum EventName
{
    /// <summary>
    /// 技能效果
    /// </summary>
    Attack,
    /// <summary>
    /// 特效
    /// </summary>
    QuartzAction,
    /// <summary>
    /// 受击
    /// </summary>
    HitAction,
    /// <summary>
    /// 停止受击
    /// </summary>
    EndAction,
    /// <summary>
    /// 瞬移
    /// </summary>
    FlashMove,
    /// <summary>
    /// 音效
    /// </summary>
    PlayMusic,
    /// <summary>
    /// 震动
    /// </summary>
    ShakeAction,
    /// <summary>
    /// 技能结束
    /// </summary>
    EndSkill
}
