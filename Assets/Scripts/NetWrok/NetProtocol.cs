using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 网络协议
/// </summary>
public class NetProtocol
{
    //开始连接
    public const int CONNECT = 100;
    //准备
    public const int READY = 101;
    //关键帧同步
    public const int SYNC_KEY = 103;
    //开始游戏
    public const int START = 104;
    //ping
    public const int PING = 200;
    //登录发送
    public const int LOGIN_REQ = 1001;
    //登录返回
    public const int LOGIN_RET = 1002;
    //匹配发送
    public const int MATCH_REQ = 1003;
    //匹配返回
    public const int MATCH_RET = 1004;
    //匹配成功返回
    public const int MATCH_SUCCESS_RET = 1005;
    //进入匹配房间
    public const int MATCH_JOIN_ROOM_REQ = 1006;
    //进入匹配房间返回
    public const int MATCH_JOIN_ROOM_RET = 1007;
    //进入选择英雄界面
    public const int MATCH_HERO_ROOM_RET = 1008;
    //点亮匹配头像
    public const int MATCH_JOIN_ROOM_IN_POSTION = 1009;
}
