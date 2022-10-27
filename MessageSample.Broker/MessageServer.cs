using System.Net;
using System.Net.Sockets;

namespace MessageSample.Broker
{
    internal class MessageServer
    {
        public static void Start()
        {
            MessageBroker broker = new();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8100);
            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            Console.WriteLine(@"
            =================================================================================
                   Message Broker Server Started listening requests at: {0}:{1}
            =================================================================================",
            endPoint.Address, endPoint.Port);

            while (true) //keep waiting to accept new connections
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine($"{client?.Client.RemoteEndPoint?.ToString()} connected at {DateTime.UtcNow}");
            }
        }
    }
}