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

        bool inOrder = true;
        int ttl = -1;

        public UdtState State
        {
            get { return UDT.GetSockState(this.handle); }
        }

        static UdtSocket()
        {
            E(UDT.Startup());
        }

        public bool InOrder
        {
            get { return this.inOrder; }
            set { this.inOrder = value; }
        }

        public int TimeToLive
        {
            get { return this.ttl; }
            set { this.ttl = value; }
        }

        private UdtSocket(UdtSockHandle handle)
        {
            this.handle = handle;
        }

        public UdtSocket(ProtocolType protocol, SocketType socketType)
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

            return new UdtSocket(h);
        }

        public void Connect(IPEndPoint remoteEndpoint)
        {
            CheckAddrVer(remoteEndpoint.AddressFamily);

            SockAddr addr = new SockAddr(remoteEndpoint);
            E(UDT.Connect(this.handle, ref addr, addr.Size));
        }

        public int Send(byte[] buf, int length)
        {
            return UDT.Send(this.handle, buf, length, 0);
        }

        public int Receive(byte[] buf)
        {
            return UDT.Recv(this.handle, buf, buf.Length, 0);
        }

        public void SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            this.SendBytes(bytes, bytes.Length);
        }

        public string ReceiveMessage(byte[] buf)
        {
            int len = this.ReceiveBytes(buf);
            if (len < 0)
            {
                return null;
            }

            return Encoding.UTF8.GetString(buf, 0, len);
        }

        public void SendBytes(byte[] buf, int length)
        {
            int len = UDT.SendBytes(this.handle, buf, length, this.ttl, this.inOrder);
            if (len != length)
            {
                throw new UdtException();
            }
        }

        public int ReceiveBytes(byte[] buf)
        {
            return UDT.RecvBytes(this.handle, buf, buf.Length);
        }

        public void Dispose()
        {
            UDT.Close(this.handle);
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
