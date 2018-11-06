using System;

public sealed class DataDefine
{
    public DataDefine()
    {
    }
    public static bool isUpdate = true;
    public const bool FubenServer = false;
    public const float netTimeCancel = 600;
#if UNITY_EDITOR
    public const string SERVER_RES_URL = "http://192.168.3.251/res/src/";
#elif UNITY_ANDROID || UNITY_IPHONE
    public const string SERVER_RES_URL = "http://api.tianyuonline.cn/res/src/";
#endif

    #region 打包需要修改的参数

    //内部渠道号：    1000
    //奇虎360：       1001
    //阿里九游：      1002
    //百度：          1003
    //腾讯应用宝：    1004
    //小米：          1005
    //VIVO:           1006
    //OPPO:           1007
    //华为：          1008
    //IOS:            2000
    //Iris:           2001

#if UNITY_EDITOR
    public const int mainchannel = 1000;//在出 IOS 渠道包时需手动将此值 改为 2000，其他渠道使用默认 1000 
#elif UNITY_ANDROID && !UNITY_EDITOR
	public const int mainchannel = 1000;  
#elif UNITY_IPHONE && !UNITY_EDITOR
	public const int mainchannel = 2000;      
#endif
    //测试热更
    //区分内外网 true：外网 false:内网
    public static bool isOutLine = false;
    public static string ver = "0";//版本,用于服务器服务器区别发送哪个区,0代表平台所有,
    public const string version = "1.2.0608";
    public const double dynamicVersion = 1.0;//是否做热更比对
    public const string ClientVersion = "1.2.0608";
    public static DebugLevel isPrintDebugInfo = DebugLevel.None;//是否打印GameDebug类型的Log
    public static bool showFPS = false;//是否显示FPS和Version开关 王豆豆添加
    public static bool isSkipFunction = false;//是否解锁所有功能
    public static bool isIgnoreSkillCd = false;   // 是否忽略技能cd
    public static bool isShowView = false;//是否显示编辑器界面GUI内容 王豆豆添加
    public static bool isShowLogOnScreen = false;//是否将GameDebugLog打印在屏幕
    public static bool isShowPing = false;//是否显示Ping
    public static bool isOpenLog = false;//是否开启LOG
    public static bool isSwitchAccount = false;//渠道平台选服界面是否有切换账号功能    
    public static bool isRecharge = true;//是否开放充值功能
    public static bool isShowNoRechargeTip = false;//是否提示充值未开放，（仅华为为TRUE）
    public static bool shareSuccess = false;//是否分享成功

    // public const string ClientVersion = "1.2.0110";
    public const string datakey = "bloodgod20160516";
    public const byte isEFS = 0;//是否加密处理
    public const int MAX_PACKET_SIZE = 1024 * 200;

    public const bool isConectSocket = true;  //否联网版本

    public static bool isLogMsg = false;  // 是否输出消息收发
    public static bool isLogMsgDetail = false;  // 是否输出消息详情
    public static bool isLogWalkMsg = false;  // 是否输出行走消息
    //是否显示FPS
    public const bool isShowFPS = false;

    #endregion

    #region ShareSDk PlatformSettings
    public static int ShareWechat = 1;
    public static int ShareWechatMoments = ShareWechat + 1;
    public static int ShareSinaWeibo = ShareWechatMoments + 1;
    public static int ShareQQ = ShareSinaWeibo + 1;
    public static int ShareQZone = ShareQQ + 1;
    #endregion

    public const string inserverip = "http://192.168.3.251/res/";
    public const string outserverip = "http://api.tianyuonline.cn/res/";
    //public const string ServerListOutLineUrl = "http://114.215.78.17:8090/ac/serverList.action?account={0}&&types={1}";http://192.168.3.251/api/servelist?account=账户&types=登录类型
    public const string ServerListOutLineUrl = "http://api.tianyuonline.cn/api/serverlist";//?account={0}&&types={1}"
    public const string ServerListUrl = "http://192.168.3.251/api/serverlist";//?account={0}&&types={1}"
    public const string SingleServerInfoUrl = "http://192.168.3.251/api/srvinfo";//获取单个服务器的信息
    public const string SingleServerInfoOutLineUrl = "http://api.tianyuonline.cn/api/srvinfo";
    public const string SingleChangeSeverInfoUrl = "http://api.tianyuonline.cn/api/serverlist-game";//获取服务器分配的地址
    //public const string LoginOutLineUrl = "http://www.tianyuonline.cn/mp/ac/Login.php?mobile={0}&passwd={1}";http://192.168.3.251/api/login?account=账户&passwd=密码&types=登录类型
    public const string LoginOutLineUrl = "http://api.tianyuonline.cn/api/login";//?account={0}&&passwd={1}";//"http://114.215.78.17/mp/ac/Login.php?mobile={0}&passwd={1}";
    public const string LoginOutLineUrl360 = "http://api.tianyuonline.cn/api/extendaccount";
    public const string LoginUrl = "http://192.168.3.251/api/login";//?account={0}&&passwd={1}&&types={2}"

    public const string RegistOutLineUrl = "http://api.tianyuonline.cn/api/reg?mobile={0}&passwd={1}&cv={2}&udid={3}+&mc={4}";//"http://114.215.78.17/mp/ac/Reg.php?mobile={0}&passwd={1}";
    public const string RegistLineUrl = "http://192.168.3.251/api/reg?mobile={0}&passwd={1}&cv={2}&udid={3}+&mc={4}";//
    public const string IOSPHPURL = "http://api.tianyuonline.cn/platsdk/ios-pre-order";
    public const string IOSCHECKURL = "http://api.tianyuonline.cn/platsdk/ios-callback";

    public static bool filterWalkMsg(uint msgId)
    {
        return !(msgId == 8449 || msgId == 261 || msgId == 260 || msgId == 259) || isLogWalkMsg;
    }
}

