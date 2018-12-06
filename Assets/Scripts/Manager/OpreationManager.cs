
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
        packet.Add("framecount", frameKeyData.m_FrameCount);
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
        GameData.m_CurrentRoleId = UnityEngine.Random.Range(10000, 1000000);
        packet.Add("msgid", NetProtocol.READY);
        packet.Add("frameCount", GameData.m_GameFrame);
        packet.Add("roleid", GameData.m_CurrentRoleId);
        packet.Add("heroid", GameData.m_HeroId);
        CWritePacket writePacket = new CWritePacket(NetProtocol.READY);
        StringBuilder builder = Jsontext.WriteData(packet);
        string json_Str = builder.ToString();
        writePacket.WriteString(json_Str);
        return writePacket;
    }
}
