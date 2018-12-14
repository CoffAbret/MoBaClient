
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class OpreationManager
{
    /// <summary>
    /// 输入操作
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public CWritePacket InputCmd(Cmd cmd, string parameter = null)
    {
        FrameKeyData frameKeyData = new FrameKeyData();
        frameKeyData.m_KeyDataList = new List<KeyData>();
        KeyData data = new KeyData();
        data.m_RoleId = GameData.m_CurrentRoleId;
        data.m_Cmd = (int)cmd;
        data.m_Parameter = parameter;
        frameKeyData.m_KeyDataList.Add(data);
        Dictionary<string, object> packet = new Dictionary<string, object>();
        packet.Add("msgid", NetProtocol.SYNC_KEY);
        packet.Add("roleid", GameData.m_CurrentRoleId);
        packet.Add("mobaKey", GameData.m_MobaKey);
        packet.Add("keydatalist", frameKeyData.m_KeyDataList);
        CWritePacket writePacket = new CWritePacket(NetProtocol.SYNC_KEY);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        return writePacket;
    }
    /// <summary>
    /// 准备操作
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public CWritePacket InputReady()
    {
        IDictionary<string, object> packet = new Dictionary<string, object>();
        packet.Add("msgid", NetProtocol.READY);
        packet.Add("playerId", GameData.m_CurrentRoleId);
        packet.Add("heroId", GameData.m_HeroId);
        packet.Add("mobaKey", GameData.m_MobaKey);
        CWritePacket writePacket = new CWritePacket(NetProtocol.READY);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        return writePacket;
    }

    /// <summary>
    /// 登录游戏
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public CWritePacket InputLogin(string account)
    {
        IDictionary<string, object> packet = new Dictionary<string, object>();
        packet.Add("msgid", NetProtocol.LOGIN_REQ);
        packet.Add("account", account);
        CWritePacket writePacket = new CWritePacket(NetProtocol.LOGIN_REQ);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        return writePacket;
    }

    /// <summary>
    /// 参加匹配
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public CWritePacket InputMatch(int matchType)
    {
        IDictionary<string, object> packet = new Dictionary<string, object>();
        packet.Add("msgid", NetProtocol.MATCH_REQ);
        packet.Add("playerId", GameData.m_CurrentRoleId);
        packet.Add("matchType", matchType);
        CWritePacket writePacket = new CWritePacket(NetProtocol.MATCH_REQ);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        return writePacket;
    }

    /// <summary>
    /// 进入匹配房间
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    public CWritePacket InputJoinMatchRoom()
    {
        IDictionary<string, object> packet = new Dictionary<string, object>();
        packet.Add("msgid", NetProtocol.MATCH_JOIN_ROOM_REQ);
        packet.Add("playerId", GameData.m_CurrentRoleId);
        packet.Add("matchKey", GameData.m_MatchKey);
        CWritePacket writePacket = new CWritePacket(NetProtocol.MATCH_JOIN_ROOM_REQ);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        return writePacket;
    }
}
