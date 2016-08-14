using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LibUdt
{
    public class UdtSocket : IDisposable
    {
        readonly UdtSockHandle handle;
        readonly AddressFamily af;
        StringBuilder buf;

        public UdtState State
        {
            get { return UDT.GetSockState(this.handle); }
        }

        static UdtSocket()
        {
            E(UDT.Startup());
        }

        private UdtSocket(UdtSockHandle handle, int bufSize)
        {
            this.handle = handle;
            this.buf = new StringBuilder(bufSize);
        }

        public UdtSocket(ProtocolType protocol, SocketType socketType, int bufSize = 4096)
        {
            if (protocol == ProtocolType.IPv4)
            {
                this.af = AddressFamily.InterNetwork;
            }
            else if (protocol == ProtocolType.IPv6)
            {
                this.af = AddressFamily.InterNetworkV6;
            }
            else
            {
                throw new ArgumentException("protocol", new NotSupportedException("Only support IPv4 and IPv6"));
            }

            this.handle = E(UDT.CreateSocket(this.af, socketType, protocol));
            this.buf = new StringBuilder(bufSize);
        }

        public void Bind(IPEndPoint localEndpoint)
        {
            CheckAddrVer(localEndpoint.AddressFamily);

            SockAddr addr = new SockAddr(localEndpoint);
            E(UDT.Bind(this.handle, ref addr, addr.Size));
        }

        public void Listen(int backlog)
        {
            E(UDT.Listen(this.handle, backlog));
        }

        public UdtSocket Accept(out IPEndPoint remoteEndpoint)
        {
            SockAddr addr;
            int addrLen;

            UdtSockHandle h = E(UDT.Accept(this.handle, out addr, out addrLen));
            Debug.Assert(addrLen == addr.Size);

            remoteEndpoint = addr.ToIPEndPoint();
            CheckAddrVer(remoteEndpoint.AddressFamily);

            return new UdtSocket(h, this.buf.Capacity);
        }

        public void Connect(IPEndPoint remoteEndpoint)
        {
            CheckAddrVer(remoteEndpoint.AddressFamily);

            SockAddr addr = new SockAddr(remoteEndpoint);
            E(UDT.Connect(this.handle, ref addr, addr.Size));
        }

        public void Send(byte[] bytes)
        {
            E(UDT.Send(this.handle, bytes, bytes.Length, 0));
        }

        public void Receive(byte[] bytes, int length)
        {
            E(UDT.Recv(this.handle, bytes, length, 0));
        }

        public void SendMessage(string message)
        {
            E(UDT.SendMsg(this.handle, message, message.Length, -1, true));
        }

        public string ReceiveMessage()
        {
            buf.Clear();
            E(UDT.RecvMsg(this.handle, buf, buf.Capacity));

            return buf.ToString();
        }

        public void Dispose()
        {
            E(UDT.Close(this.handle));
        }

        void CheckAddrVer(AddressFamily af)
        {
            if (this.af != af)
            {
                throw new UdtException(
                    string.Format("The AddressFamily {0} is different from the AddressFamily of this socket", af.ToString()));
            }
        }

        static void E(int ret)
        {
            if (ret != 0)
            {
                throw new UdtException();
            }
        }

        static UdtSockHandle E(UdtSockHandle h)
        {
            if ((int)h == -1)
            {
                throw new UdtException();
            }

            return h;
        }
    }
}
