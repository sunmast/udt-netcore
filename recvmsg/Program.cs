using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using LibUdt;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (UdtSocket server = new UdtSocket(ProtocolType.IPv4, SocketType.Stream))
            {
                server.Bind(new IPEndPoint(IPAddress.Loopback, 8888));
                server.Listen(10);

                IPEndPoint remoteEp;
                using (UdtSocket us = server.Accept(out remoteEp))
                {
                    byte[] buf = new byte[1024];
                    us.Receive(buf, 4);

                    int length = BitConverter.ToInt32(buf, 0);
                    us.Receive(buf, length);

                    string msg = Encoding.UTF8.GetString(buf, 0, length);
                    Console.WriteLine(msg);

                    Console.ReadLine();
                }
            }
        }
    }
}
