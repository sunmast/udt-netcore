using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace LibUdt
{
    public class UdtSocket : IDisposable
    {
        readonly UdtSockHandle u;

        static UdtSocket()
        {
            API.Startup();
        }

        public UdtSocket(ProtocolType protocol)
        {
            if (protocol == ProtocolType.IPv4)
            {
                u = API.CreateSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
            }
            else if(protocol == ProtocolType.IPv6)
            {
                u = API.CreateSocket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.IPv6);
            }
            else
            {
                throw new ArgumentException("protocol", new NotSupportedException("Only supports IPv4 and IPv6"));
            }
        }

        public void Dispose()
        {
            API.Close(u);
        }
    }
}
