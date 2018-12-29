using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KCP
{
    //运行标记
    private bool running = false;
    //发送窗口大小
    private int IKCP_SEND_WND = 32;
    //接收窗口大小
    private int IKCP_RECV_WND = 32;
    //kcp头长度
    public static byte IKCP_OVERHEAD = 22;
    //发送包
    public static byte IKCP_CMD_PUSH = 1;
    //确认包
    public static byte IKCP_CMD_ACK = 2;
    //重传时间
    private int IKCP_RTO_MIN = 100;
    private int IKCP_RTO_MAX = 1000 * 60;
    //输入队列
    private Queue<KcpPacket> received;
    //输出队列
    private Queue<KcpPacket> sendList;
    //待发送的消息队列
    private List<KcpPacket> sndQueue = new List<KcpPacket>();
    //接收消息的队列
    private List<KcpPacket> rcvQueue = new List<KcpPacket>();
    // 发送缓存  ，数据的sn 可能是间隔的
    private Dictionary<int, KcpPacket> sndBuf = new Dictionary<int, KcpPacket>();
    //接收缓存  ，数据的sn 可能是间隔的
    private Dictionary<int, KcpPacket> rcvBuf = new Dictionary<int, KcpPacket>();
    //确认包数据  SN
    private List<int> ackSnList = new List<int>();
    //确认包数据  TS
    private List<long> ackTsList = new List<long>();

    //发送窗口大小
    private int sendWnd = 0;
    //接收窗口大小
    private int recvWnd = 0;
    //发送的最大顺序号
    private int sendNext = 0;
    //接收的最大顺序号
    private int recvNext = 0;
    //远端最大连续确认号
    private int remoteSn = 0;
    //远端最小连续确认号
    private int remoteMinSn = 0;
    //当前时间
    private long currTime = 0;
    //kcp 起动的时间，超过1分钟没动作过就关闭
    private long kcpStartTime = 0;
    //kcp 起动的时间，超过1分钟没动作过就关闭
    private long kcpEndTime = 0;
    //rtt 平均时间 (ms)
    private int rxSrtt = 0;
    //rtt时间间隔 (ms)
    private int rxRttval = 0;
    //rto时间 ms
    private int rxRto = 0;

    public KCP()
    {
        this.received = new Queue<KcpPacket>();
        this.sendList = new Queue<KcpPacket>();
        this.sendWnd = IKCP_SEND_WND;
        this.recvWnd = IKCP_RECV_WND;
        this.running = true;
        this.sendNext = 0;
        this.recvNext = 0;
        this.kcpStartTime = GetTimestamp();
        this.kcpEndTime = GetTimestamp();
    }

    public void update()
    {
        if (!GameData.m_IsClick)
            return;
        currTime = GetTimestamp();
        //接收的数据待处理
        while (received.Count > 0)
        {
            KcpPacket kpt = received.Dequeue();
            rcvQueue.Add(kpt);
        }
        //本地输入待处理
        while (sendList.Count > 0)
        {
            KcpPacket kpt = sendList.Dequeue();
            sndQueue.Add(kpt);
        }
        //处理要接收的
        processRecv();
        //处理要发的确认包
        processAck();
        //处理要发送的	
        processSend();
        //处理状态
        processStatus();
    }

    public void processRecv()
    {
        int count = rcvQueue.Count;
        while (rcvQueue.Count > 0)
        {
            KcpPacket kpt = rcvQueue[0];
            rcvQueue.RemoveAt(0);
            if (kpt.m_Cmd == IKCP_CMD_PUSH)
            {
                if (kpt.m_Sn >= recvNext)
                { //必需要大等于最后一个可收号才是有效的					
                    rcvBuf.Add(kpt.m_Sn, kpt);
                    checkSendWnd(kpt.m_Win);
                    //加到确认列表中
                    addAckList(kpt.m_Sn, kpt.m_Ts);
                    recvWnd--;
                    recvWnd = recvWnd < 0 ? 1 : recvWnd;
                }
            }
            else if (kpt.m_Cmd == IKCP_CMD_ACK)
            {
                parseAck(kpt);
                checkSendWnd(kpt.m_Win);
            }

            if (kpt.m_NextSn > remoteSn)
            {
                remoteSn = kpt.m_NextSn;
            }
        }
        for (int i = 0; i < count; i++)
        {

        }

        if (count > 0)
        {
            setLastActiveTime(currTime);
        }

        //将数据包顺序加入到结果中 , 并加到确认列表中
        //		for (KcpPacket kpt : rcvBuf.values()){			
        //			addAckList(kpt.getSn() , kpt.getTs());
        //		}
        count = rcvBuf.Count;
        for (int i = recvNext; i < recvNext + count; i++)
        {
            KcpPacket kpt = rcvBuf[recvNext];
            if (kpt != null)
            {
                rcvBuf.Remove(kpt.m_Sn);
                recvKcp(kpt);
                checkSendWnd(kpt.m_Win);
                recvNext++;
            }
        }
        //设置当前窗口大小
        recvWnd = IKCP_SEND_WND - rcvBuf.Count;
        if (recvWnd == 0)
        {
            recvWnd = 1;
        }
    }

    public void processAck()
    {
        int count = ackSnList.Count;
        if (count <= 0) return;
        IoBuffer io = new IoBuffer();
        for (int i = 0; i < count; i++)
        {
            io.WriteInt(ackSnList[i]);//sn
            io.WriteLong(ackTsList[i]);//ts
        }
        io.SeekBegin();
        byte[] data = io.ReadBytes((int)io.RemainingBytes());

        KcpPacket kpt = new KcpPacket();
        kpt.m_Cmd = KCP.IKCP_CMD_ACK;
        kpt.m_Data = data;
        kpt.m_Win = (byte)recvWnd;
        kpt.m_NextSn = recvNext;
        sendKcp(kpt);
        ackSnList.Clear();
        ackTsList.Clear();
    }

    public void processSend()
    {
        //sendWnd = sendWnd == 0 ? 1 : sendWnd;
        // 按窗口大小处理包
        if (sendWnd > 0)
        {
            int count = sndQueue.Count;
            while (sndQueue.Count > 0)
            {
                if (sendWnd > 0)
                {
                    KcpPacket kpt = sndQueue[0];
                    sndQueue.RemoveAt(0);
                    if (kpt.m_Cmd == IKCP_CMD_PUSH)
                    {
                        kpt.m_Sn = sendNext;
                        kpt.m_Ts = currTime;
                        sndBuf.Add(kpt.m_Sn, kpt);
                        sendNext++;
                        sendWnd--;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < count; i++)
            {

            }
        }
        //已经确认，不用再重发的包sn
        List<int> removeSnLst = new List<int>();
        //处理发送，重发
        UnityEngine.Debug.LogError("数组长度:" + sndBuf.Count);
        foreach (KeyValuePair<int, KcpPacket> item in sndBuf)
        {
            UnityEngine.Debug.LogError("数组长度:" + item.Value.m_Sn);
            item.Value.m_Win = (byte)recvWnd;//这里是告之接收方，当前发送方的可接收窗口大小
            item.Value.m_NextSn = recvNext;
            //如果序号小于远程最大确认包，说明这个确认晚了，远程已经有此包，不用重发
            if (item.Value.m_Sn < remoteSn)
            {
                removeSnLst.Add(item.Value.m_Sn);
                continue;
            }
            if (item.Value.m_Ts >= currTime && item.Value.m_Rto == 0)
            {//新包直接发送
                sendKcp(item.Value);
                //UnityEngine.Debug.LogError(string.Format("发送：m_Sn:{0},m_Ts:{1}", item.Value.m_Sn, item.Value.m_Ts));
            }
            else
            {
                if (remoteSn < item.Value.m_Sn)
                {
                    if (item.Value.m_Skip >= 3)
                    {
                        item.Value.m_Ts = currTime;
                        item.Value.m_Skip = 0;
                        sendKcp(item.Value);
                        //UnityEngine.Debug.LogError(string.Format("重发：m_Sn:{0},m_Ts:{1}", item.Value.m_Sn, item.Value.m_Ts));
                        continue;
                    }
                }
                //收到对特定报文段的确认之前计时器超时，则重传该报文，并且进行RTO = 2 * RTO进行退避。这是TCP的，咱们使用网上给出的值1.5
                if (rxRto == 0) rxRto = 1000;
                long time = (long)(1.5 * (item.Value.m_Rto + 1) * rxRto);
                time = time > IKCP_RTO_MAX ? IKCP_RTO_MAX : time;
                time = time == 0 ? IKCP_RTO_MIN : time;
                //超时重发
                if (currTime > time + item.Value.m_Ts)
                {
                    item.Value.m_Ts = currTime;
                    sendKcp(item.Value);
                    //UnityEngine.Debug.LogError(string.Format("超时重发：m_Sn:{0},m_Ts:{1}", item.Value.m_Sn, item.Value.m_Ts));
                    item.Value.IncreaRto();
                    item.Value.m_Skip = 0;
                }

            }
        }

        for (int i = 0; i < removeSnLst.Count; i++)
        {
            sndBuf.Remove(removeSnLst[i]);
        }
    }

    public void processStatus()
    {
        if (currTime - this.kcpEndTime > 1000 * 60)
        {
            this.running = false;
            GameData.m_GameManager.m_NetManager.m_UdpClient.Disconnect();
        }
    }

    public void send(CWritePacket pk)
    {
        KcpPacket kpt = new KcpPacket();
        kpt.m_Cmd = KCP.IKCP_CMD_PUSH;
        kpt.m_Data = pk.GetPacketByte();
        sendList.Enqueue(kpt);
    }

    /// <summary>
    /// 给应用层的收包接口，实际收到由KCP来控制
    /// </summary>
    /// <param name="pk"></param>
    public void recv(KcpPacket pk)
    {
        received.Enqueue(pk);
    }

    /// <summary>
    /// 实际调用发送
    /// </summary>
    /// <param name="pk"></param>
    public void sendKcp(KcpPacket pk)
    {
        if (GameData.m_GameManager.m_NetManager.m_UdpClient == null)
            return;
        GameData.m_GameManager.m_NetManager.m_UdpClient.AsyncSendKcpData(pk);
    }


    private void checkSendWnd(int wnd)
    {
        if (sendWnd < wnd)
        {
            sendWnd = wnd;
        }
    }

    /// <summary>
    ///接收有效数据包后 ，将sn,ts 添加到确认队列，后面会回复给发送方以便确认。
    /// </summary>
    /// <param name="sn"></param>
    /// <param name="ts"></param>
    public void addAckList(int sn, long ts)
    {
        ackSnList.Add(sn);
        ackTsList.Add(ts);
    }

    /// <summary>
    /// 收到确认包
    /// </summary>
    private void parseAck(KcpPacket kpt)
    {
        long minTs = 0;
        IoBuffer io = new IoBuffer(kpt.m_Data);
        while (io.RemainingBytes() > 0)
        {
            int sn = io.ReadInt();
            long ts = io.ReadLong();
            sndBuf.Remove(sn); // 将确认的包从待发队列中去掉
            checkSkip(sn);//检查跳过包
            if (minTs == 0 || minTs < ts)
            {
                minTs = ts;
            }
        }
        if (currTime - minTs >= 0)
        {
            updateRto((int)(currTime - minTs));
        }
    }

    /// <summary>
    /// 检查跳过的包
    /// </summary>
    /// <param name="sn"></param>
    public void checkSkip(int sn)
    {
        foreach (KeyValuePair<int, KcpPacket> item in sndBuf)
        {
            if (item.Value.m_Sn < sn)
                item.Value.IncreaSkip();
        }
    }

    /// <summary>
    /// 设置最好活跃时间
    /// </summary>
    /// <param name="time"></param>
    private void setLastActiveTime(long time)
    {
        this.kcpEndTime = time;
    }

    /// <summary>
    /// 实际调用发送
    /// </summary>
    /// <param name="kpt"></param>
    public void recvKcp(KcpPacket kpt)
    {
        List<CReadPacket> lst = kpt.Packet;
        if (lst != null)
        {
            foreach (CReadPacket item in lst)
            {
                GameData.m_GameManager.m_NetManager.m_UdpClient.Receive(item);
            }
        }
    }

    /// <summary>
    /// 计算rto 时间 
    /// </summary>
    /// <param name="rtt"></param>
    private void updateRto(int rtt)
    {
        if (rxSrtt == 0)
        {
            rxSrtt = rtt;
            rxRttval = rtt / 2;//rtt 一来一回的时间ms
        }
        else
        {
            long delta = rtt - rxSrtt;
            if (delta < 0)
            {
                delta = -delta;
                delta = 33;
            }

            //          由于路由器的拥塞和端系统负载的变化，由于这种波动，用一个报文段所测的SampleRTT来代表同一段时间内的RTT总是非典型的，为了得到一个典型的RTT，
            //          TCP规范中使用低通过滤器来更新一个被平滑的RTT估计器。
            //          TCP维持一个估计RTT（称之为EstimatedRTT），一旦获得一个新SampleRTT时，则根据下式来更新EstimatedRTT： 
            //          EstimatedRTT = （1-a）* EstimatedRTT + a * SampleRTT 
            //          其中a通常取值为0.125，即：
            //          EstimatedRTT = 0.875 * EstimatedRTT + 0.125 * SampleRTT
            //          每个新的估计值的87.5%来自前一个估计值，而12.5%则取自新的测量。            
            rxSrtt = (7 * rxSrtt + rtt) / 8;

            //          由于新测量SampleRTT的权值只占EstimatedRTT的12.5%，当实际RTT变化很大的时候，即便测量到的SampleRTT变化也很大，但是所占比重小，
            //          最后EstimatedRTT的变化也不大，从而RTO的变化不大，造成RTO过小，容易引起不必要的重传。因此对RTT的方差跟踪则显得很有必要。 
            //          在TCP规范中定义了RTT偏差DevRTT，用于估算SampleRTT一般会偏离EstimatedRTT的程度：
            //          DevRTT = (1-B)*DevRTT + B*|SampleRTT - EstimatedRTT|
            //          其中B的推荐值为0.25，当RTT波动很大的时候，DevRTT的就会很大。
            rxRttval = (int)((3 * rxRttval + delta) / 4);

            if (rxSrtt < 1)
            {
                rxSrtt = 1;
            }
        }
        //      重传时间间隔RTO的计算公式为：
        //      RTO = EstimatedRTT + 4 * DevRTT
        int rto = rxSrtt + Math.Max(IKCP_RTO_MIN, 4 * rxRttval);
        rxRto = ibound(IKCP_RTO_MIN, rto, IKCP_RTO_MAX);
    }

    private int ibound(int lower, int middle, int upper)
    {
        return Math.Min(Math.Max(lower, middle), upper);
    }


    public long GetTimestamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}
