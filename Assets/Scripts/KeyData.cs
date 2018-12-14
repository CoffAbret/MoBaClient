using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 每一帧操作
/// </summary>
public class FrameKeyData
{
    //第几帧操作
    public int m_FrameCount;
    //操作List
    public List<KeyData> m_KeyDataList;
}

/// <summary>
/// 操作
/// </summary>
public class KeyData
{
    //操作人
    public int m_RoleId;
    //操作命令
    public int m_Cmd;
    //操作参数
    public string m_Parameter;
}

/// <summary>
/// 匹配用户数据
/// </summary>
public class MatchPlayerData
{
    public int m_PlayerId;
    public string m_PlayerName;
    public int m_CampId;
    public int m_Pos;
}
