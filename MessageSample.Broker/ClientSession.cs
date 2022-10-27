using MessageSample.Share;
using System.Net.Sockets;
using System.Text;

namespace MessageSample.Broker
{
    /// <summary>
    /// Client session object: Receiver for queue or subcriber for topic
    /// </summary>
    internal class ClientSession
    {
        public TcpClient? Client { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public SubcribeStorageInfo? SubcribeStorage { get; set; }
        public DateTime ConnectedDate { get; set; } = DateTime.UtcNow;

        public ClientTypeEnum ClientType;

        private IMessageBroker? _messageBroker;

        /// <summary>
        /// Wait and process message from sender / publisher
        /// </summary>
        private void MessageHandler()
        {
            while (true)
            {
                try
                {
                    if (Client == null)
                        break;

                    const int bytesize = 1024 * 1024;
                    byte[] buffer = new byte[bytesize];
                    NetworkStream networkStream = Client.GetStream();
                    networkStream.Read(buffer, 0, Client.ReceiveBufferSize);

                    var message = buffer.GetString();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine($"{Client?.Client.RemoteEndPoint?.ToString()} sent command at {DateTime.UtcNow}");
                        Console.WriteLine($"Command: {message}\n");

                        ExtractClientInfo(message);

                        _messageBroker?.AcceptMessage(message);
                        _messageBroker?.AddSubscriber(this);
                    }
                }
                catch
                {
                    Console.WriteLine($"{Client?.Client.RemoteEndPoint?.ToString()} disconnected at {DateTime.UtcNow}");
                    _messageBroker?.RemoveSubscriber(this);
                    break;
                }
            }
        }

        private void ExtractClientInfo(string message)
        {
            ClientType = message.GetClientType();
            SubcribeStorage = new SubcribeStorageInfo
            {
                StorageName = message.GetStorage(),
                StorageType = message.GetStorageType()
            };
        }

        public void VisitBroker(IMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public void StartClient()
        {
            Thread ctThread = new(MessageHandler);
            ctThread.Start();
        }

        public string? ClientEndpoint
        {
            get
            {
                return Client?.Client.RemoteEndPoint?.ToString();
            }
        }
    }
}