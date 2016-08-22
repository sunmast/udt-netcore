# udt-netcore
A lightweight .NET wrapper for UDT (http://udt.sourceforge.net) implemented in pure C# and runs in .NET core and Mono/Microsoft .NET

##Samples


###Receive Message (server)
```C#
public static void Main(string[] args)
{
    using (UdtSocket server = new UdtSocket(ProtocolType.IPv4, SocketType.Dgram))
    {
        server.Bind(new IPEndPoint(IPAddress.Loopback, 8888));
        server.Listen(10);

        while (true)
        {
            IPEndPoint remoteEp;
            UdtSocket socket = server.Accept(out remoteEp);
            ThreadPool.QueueUserWorkItem(ReceiveMessage, new object[] { socket, remoteEp });
        }
    }
}

static void ReceiveMessage(object state)
{
    object[] objects = (object[])state;

    using (UdtSocket socket = (UdtSocket)objects[0])
    {
        IPEndPoint remoteEp = (IPEndPoint)objects[1];

        byte[] buf = new byte[100];
        string msg;

        while ((msg = socket.ReceiveMessage(buf)) != null)
        {
            Console.WriteLine("{0} from {1}:{2}", msg, remoteEp.Address, remoteEp.Port);
        }
    }

    Console.WriteLine("Worker thread {0} has exited", Thread.CurrentThread.ManagedThreadId);
}
```
###Send Message (client)
```C#
public static void Main(string[] args)
{
	using (UdtSocket client = new UdtSocket(ProtocolType.IPv4, SocketType.Dgram))
	{
	    client.Connect(new IPEndPoint(IPAddress.Loopback, 8888));

		while (true)
		{
			client.SendMessage("Hello 你好 UDT! " + DateTime.Now.ToString());
			Console.ReadLine();
		}
	}
}
```