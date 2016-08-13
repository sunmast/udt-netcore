using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace LibUdt
{
    internal enum UdtSockHandle : int { }

    internal enum UdpSockHandle : int { }

    internal enum SysSockHandle : int { }

    public enum UdtState : int
    {
        Init = 1, Opened, Listening, Connecting, Connected, Broken, Closing, Closed, NonExist
    }

    public enum UdtOption : int
    {
        MaxTransferUnit,             // the Maximum Transfer Unit
        BlockingSend,          // if sending is blocking
        BlockingRecv,          // if receiving is blocking
        CongestionAlgorithm,              // custom congestion control algorithm
        WindowSize,     // Flight flag size (window size)
        SendBuf,          // maximum buffer in sending queue
        RecvBuf,          // UDT receiving buffer size
        Linger,          // waiting for unsent data when closing
        UdpSendBuf,          // UDP sending buffer size
        UdpRecvBuf,          // UDP receiving buffer size
        MaxMsg,          // maximum datagram message size
        MsgTtl,          // time-to-live of a datagram message
        RendezvousMode,      // rendezvous connection mode
        SendTimeout,        // send() timeout
        RecvTimeout,        // recv() timeout
        ReuseAddr,  // reuse an existing port or create a new one
        MaxBandwidth,      // maximum bandwidth (bytes per second) that the connection can use
        State,      // current socket state, see UDTSTATUS, read only
        Event,      // current avalable events associated with the socket
        SendData,        // size of data in the sending buffer
        RecvData     // size of data available for recv
    }

    // public enum AF : int
    // {
    //     Unspecified = 0,
    //     IPv4 = 2,
    //     IPv6 = 23
    // }

    // public enum Protocol : int
    // {
    //     IPv4 = 4,
    //     IPv6 = 41
    // }

    // public enum SockType : int
    // {
    //     Stream = 1
    // }

    [StructLayout(LayoutKind.Sequential)]
    public struct SockAddr
    {
        public const int DataLength = 14;

        public ushort AddressFamily;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = DataLength)]
        public byte[] Data;

        public SockAddr(AddressFamily af, byte[] bytes)
        {
            this.AddressFamily = (ushort)af;
            this.Data = new byte[DataLength];

            bytes.CopyTo(this.Data, 0);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UdtPerfInfo
    {
        // global measurements

        public long msTimeStamp;                    // time since the UDT entity is started, in milliseconds
        public long pktSentTotal;                   // total number of sent data packets, including retransmissions
        public long pktRecvTotal;                   // total number of received packets
        public int pktSndLossTotal;                 // total number of lost packets (sender side)
        public int pktRcvLossTotal;                 // total number of lost packets (receiver side)
        public int pktRetransTotal;                 // total number of retransmitted packets
        public int pktSentACKTotal;                 // total number of sent ACK packets
        public int pktRecvACKTotal;                 // total number of received ACK packets
        public int pktSentNAKTotal;                 // total number of sent NAK packets
        public int pktRecvNAKTotal;                 // total number of received NAK packets
        public long usSndDurationTotal;             // total time duration when UDT is sending data (idle time exclusive)

        // local measurements

        public long pktSent;                        // number of sent data packets, including retransmissions
        public long pktRecv;                        // number of received packets
        public int pktSndLoss;                      // number of lost packets (sender side)
        public int pktRcvLoss;                      // number of lost packets (receiver side)
        public int pktRetrans;                      // number of retransmitted packets
        public int pktSentACK;                      // number of sent ACK packets
        public int pktRecvACK;                      // number of received ACK packets
        public int pktSentNAK;                      // number of sent NAK packets
        public int pktRecvNAK;                      // number of received NAK packets
        public double mbpsSendRate;                 // sending rate in Mb/s

        public double mbpsRecvRate;                 // receiving rate in Mb/s
        public long usSndDuration;                  // busy sending time (i.e., idle time exclusive)

        // instant measurements

        public double usPktSndPeriod;               // packet sending period, in microseconds
        public int pktFlowWindow;                   // flow window size, in number of packets
        public int pktCongestionWindow;             // congestion window size, in number of packets
        public int pktFlightSize;                   // number of packets on flight
        public double msRTT;                        // RTT, in milliseconds
        public double mbpsBandwidth;                // estimated bandwidth, in Mb/s
        public int byteAvailSndBuf;                 // available UDT sender buffer size
        public int byteAvailRcvBuf;                 // available UDT receiver buffer size
    }

    internal static class API
    {
        [DllImport("libudt", EntryPoint = "udt_startup")]
        internal static extern int Startup();

        [DllImport("libudt", EntryPoint = "udt_cleanup")]
        internal static extern int Cleanup();

        [DllImport("libudt", EntryPoint = "udt_socket")]
        internal static extern UdtSockHandle CreateSocket(AddressFamily af, SocketType type, ProtocolType protocol);

        [DllImport("libudt", EntryPoint = "udt_bind")]
        internal static extern int Bind(UdtSockHandle u, ref SockAddr name, int namelen);

        [DllImport("libudt", EntryPoint = "udt_listen")]
        internal static extern int Listen(int u, int backlog);

        [DllImport("libudt", EntryPoint = "udt_accept")]
        internal static extern UdtSockHandle Accept(UdtSockHandle u, out SockAddr name, out int namelen);

        [DllImport("libudt", EntryPoint = "udt_connect")]
        internal static extern int Connect(UdtSockHandle u, ref SockAddr name, int namelen);

        [DllImport("libudt", EntryPoint = "udt_close")]
        internal static extern int Close(UdtSockHandle u);

        [DllImport("libudt", EntryPoint = "udt_getpeername")]
        internal static extern int GetPeerName(UdtSockHandle u, out SockAddr name, out int namelen);

        [DllImport("libudt", EntryPoint = "udt_getsockname")]
        internal static extern int GetSockName(UdtSockHandle u, out SockAddr name, out int namelen);

        [DllImport("libudt", EntryPoint = "udt_getsockopt")]
        internal static extern int GetSockOpt(UdtSockHandle u, int level, UdtOption optname, out byte[] optval, out int optlen);

        [DllImport("libudt", EntryPoint = "udt_setsockopt")]
        internal static extern int SetSockOpt(UdtSockHandle u, int level, UdtOption optname, ref byte[] optval, int optlen);

        [DllImport("libudt", EntryPoint = "udt_send")]
        internal static extern int Send(UdtSockHandle u, byte[] buf, int len, int flags);

        [DllImport("libudt", EntryPoint = "udt_recv")]
        internal static extern int Recv(UdtSockHandle u, byte[] buf, int len, int flags);

        [DllImport("libudt", EntryPoint = "udt_sendmsg", CharSet = CharSet.Ansi)]
        internal static extern int SendMsg(UdtSockHandle u, string buf, int len, int ttl = -1, bool inorder = false);

        [DllImport("libudt", EntryPoint = "udt_recvmsg", CharSet = CharSet.Ansi)]
        internal static extern int RecvMsg(UdtSockHandle u, StringBuilder buf, int len);

        // [DllImport("libudt", EntryPoint = "udt_sendfile")]
        // internal static extern long SendFile(UDTSOCKET u, std::fstream& ifs, long& offset, long size, int block = 364000);

        // [DllImport("libudt", EntryPoint = "udt_recvfile")]
        // internal static extern long RecvFile(UDTSOCKET u, std::fstream& ofs, long& offset, long size, int block = 7280000);

        [DllImport("libudt", EntryPoint = "udt_sendfile2")]
        internal static extern long SendFile(UdtSockHandle u, string path, ref long offset, long size, int block = 364000);

        [DllImport("libudt", EntryPoint = "udt_recvfile2")]
        internal static extern long RecvFile(UdtSockHandle u, string path, ref long offset, long size, int block = 7280000);

        [DllImport("libudt", EntryPoint = "udt_epoll_create")]
        internal static extern int EpollCreate();

        // [DllImport("libudt", EntryPoint = "udt_epoll_add_usock")]
        // internal static extern int EpollAddUdtSock(int eid, UdtSocketHandle u, const int* events = NULL);

        // [DllImport("libudt", EntryPoint = "udt_epoll_add_ssock")]
        // internal static extern int EpollAddSysSock(int eid, SysSocketHandle s, const int* events = NULL);

        [DllImport("libudt", EntryPoint = "udt_epoll_remove_usock")]
        internal static extern int EpollRemoveUdtSock(int eid, UdtSockHandle u);

        [DllImport("libudt", EntryPoint = "udt_epoll_remove_ssock")]
        internal static extern int EpollRemoveSysSock(int eid, SysSockHandle s);

        // [DllImport("libudt", EntryPoint = "udt_epoll_wait")]
        // internal static extern int epoll_wait(int eid, std::set<UDTSOCKET>* readfds, std::set<UDTSOCKET>* writefds, long msTimeOut,
        //     std::set<SYSSOCKET>* lrfds = NULL, std::set<SYSSOCKET>* wrfds = NULL);

        // [DllImport("libudt", EntryPoint = "udt_epoll_wait2")]
        // internal static extern int epoll_wait2(int eid, UDTSOCKET* readfds, int* rnum, UDTSOCKET* writefds, int* wnum, long msTimeOut,
        //     SYSSOCKET* lrfds = NULL, int* lrnum = NULL, SYSSOCKET* lwfds = NULL, int* lwnum = NULL);

        [DllImport("libudt", EntryPoint = "udt_epoll_release")]
        internal static extern int EpollRelease(int eid);

        // [DllImport("libudt", EntryPoint = "udt_getlasterror")]
        // internal static extern ERRORINFO& getlasterror();

        [DllImport("libudt", EntryPoint = "udt_getlasterror_code")]
        internal static extern int GetLastErrorCode();

        [DllImport("libudt", EntryPoint = "udt_getlasterror_desc", CharSet = CharSet.Ansi)]
        internal static extern string GetLastErrorDesc();

        [DllImport("libudt", EntryPoint = "udt_perfmon")]
        internal static extern int PerfMon(UdtSockHandle u, out UdtPerfInfo perf, bool clear = true);

        [DllImport("libudt", EntryPoint = "udt_getsockstate")]
        internal static extern UdtState GetSockState(UdtSockHandle u);
    }
}
