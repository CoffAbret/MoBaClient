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
}
