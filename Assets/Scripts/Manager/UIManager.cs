using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI对象管理类
/// </summary>
public class UIManager
{
    //刷新技能图标
    public delegate void UpdateSkillUICallback(List<SkillNode> skillNodeList);
    public UpdateSkillUICallback m_UpdateSkillUICallback = null;
}
