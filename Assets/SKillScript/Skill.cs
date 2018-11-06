using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill : ScriptableObject
{
    //技能ID
    public int skillId;
    //技能动画事件
    public List<AnimaEvent> animaEvent = new List<AnimaEvent>();
}
