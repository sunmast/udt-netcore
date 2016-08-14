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
            using (UdtSocket client = new UdtSocket(ProtocolType.IPv4, SocketType.Stream))
            {
                client.Connect(new IPEndPoint(IPAddress.Loopback, 8888));

                byte[] bytes = Encoding.UTF8.GetBytes("Hello UDT!");
                int length = bytes.Length;
                
                client.Send(BitConverter.GetBytes(length));
                client.Send(bytes);

                Console.ReadLine();
            }
        }
    }
}
