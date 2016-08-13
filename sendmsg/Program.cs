using System;
using System.Net.Sockets;
using LibUdt;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var x = new UdtSocket(ProtocolType.IPv4);
            x.Dispose();
        }
    }
}
