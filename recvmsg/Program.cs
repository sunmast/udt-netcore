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
            using (UdtSocket server = new UdtSocket(ProtocolType.IPv4, SocketType.Dgram))
            {
                server.Bind(new IPEndPoint(IPAddress.Loopback, 8888));
                server.Listen(10);

                IPEndPoint remoteEp;

                while (true)
                {
                    using (UdtSocket us = server.Accept(out remoteEp))
                    {
                        string msg = us.ReceiveMessage();
                        Console.WriteLine(msg);
                    }
                }
            }
        }
    }
}
