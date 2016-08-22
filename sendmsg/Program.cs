using System;
using System.Net;
using System.Net.Sockets;
using LibUdt;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (UdtSocket client = new UdtSocket(ProtocolType.IPv4, SocketType.Dgram))
            {
                client.Connect(new IPEndPoint(IPAddress.Loopback, 8888));
                client.SendMessage("Hello UDT! " + DateTime.Now.ToString());
            }
        }
    }
}
