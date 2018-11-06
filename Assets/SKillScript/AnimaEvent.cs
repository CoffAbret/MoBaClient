using UnityEngine;

public class AnimaEvent : ScriptableObject
{
    //状态
    public Status status = Status.Prepare;
    //事件名称
    public EventName eventName = EventName.Attack;
    //事件参数
    public string eventParameter = "";
    //调用时间
    public float normalTime = 0f;
}