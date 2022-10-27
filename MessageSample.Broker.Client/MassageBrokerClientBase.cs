using MessageSample.Share;
using System.Net.Sockets;
using System.Text;

namespace MessageSample.Broker.Client
{
    public class MassageBrokerClientBase
    {
        protected TcpClient? _client;
        protected const int _bytesize = 1024 * 1024; // buffer size
        protected StorageTypeEnum _storageType;
        protected string _storageName = string.Empty;
        protected ClientTypeEnum ClientType;

        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Request to create topic storage
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        protected async Task CreateQueueIfNotExist(string queueName)
        {
            await SendMessageAsync($"{CommandActionEnum.CreateQueue}", queueName);
        }

        /// <summary>
        /// Request to create queue storage
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        protected async Task CreateTopicIfNotExist(string topicName)
        {
            await SendMessageAsync($"{CommandActionEnum.CreateTopic}", topicName);
        }

        /// <summary>
        /// Send message to broker
        /// </summary>
        /// <param name="action"></param>
        /// <param name="storage"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected async Task SendMessageAsync(string action, string? message)
        {
            if (_client == null)
                throw new Exception("Cannot connect to broker server");

            var command = $"{ClientType}|{action}|{_storageType}|{_storageName}|{message}";

            var data = Encoding.UTF8.GetBytes(command);
            await _client.GetStream().WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// start a tcp/ip connection to broker server async
        /// </summary>
        /// <returns></returns>
        protected async Task ConnectBrokerServerAsync()
        {
            while (true)
            {
                if (_client == null || !_client.Connected)
                {
                    try
                    {
                        Console.WriteLine("Connecting to server...");
                        var connectInfo = ConnectionString.Split(":");
                        _client = new TcpClient();
                        await _client.ConnectAsync(connectInfo[0], int.Parse(connectInfo[1]));
                        if (_client.Connected)
                        {
                            Console.WriteLine("Connected to Server.");
                        }
                    }
                    catch { } // ignore error and continue to keep connecting 
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// start a tcp/ip connection to broker server
        /// </summary>
        /// <returns></returns>
        protected void ConnectBrokerServer()
        {
            while (true)
            {
                if (_client == null || !_client.Connected)
                {
                    try
                    {
                        Console.WriteLine("Connecting to server...");
                        var connectInfo = ConnectionString.Split(":");
                        _client = new TcpClient();
                        _client.Connect(connectInfo[0], int.Parse(connectInfo[1]));
                        if (_client.Connected)
                        {
                            Console.WriteLine("Connected to Server.");
                        }
                    }
                    catch { } // ignore error and continue to keep connecting 
                }
                else
                {
                    break;
                }
            }
        }
    }
}