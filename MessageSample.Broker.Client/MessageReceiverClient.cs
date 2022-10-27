using MessageSample.Share;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Reflection.Metadata;

namespace MessageSample.Broker.Client
{
    public sealed class MessageReceiverClient : MassageBrokerClientBase
    {
        public Action<Message?> OnMessageReceived { get; set; } = delegate { };

        private MessageReceiverClient(string connectionString)
        {
            ClientType = ClientTypeEnum.Receiver;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Initial a queue client connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static async Task<MessageReceiverClient> GetClientQueueAsync(string connectionString, string queueName)
        {
            var client = new MessageReceiverClient(connectionString)
            {
                _storageType = StorageTypeEnum.Queue,
                _storageName = queueName
            };

            await client.ConnectBrokerServerAsync();
            await client.CreateQueueIfNotExist(queueName);

            Thread thr = new(client.OnMessageReceivedHandler);
            thr.Start();

            return client;
        }

        /// <summary>
        /// Inital a topic client connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public static async Task<MessageReceiverClient> GetClientTopicAsync(string connectionString, string topicName)
        {
            var client = new MessageReceiverClient(connectionString)
            {
                _storageType = StorageTypeEnum.Topic,
                _storageName = topicName
            };

            await client.ConnectBrokerServerAsync();
            await client.CreateTopicIfNotExist(topicName);

            Thread thr = new(client.OnMessageReceivedHandler);
            thr.Start();

            return client;
        }

        /// <summary>
        /// Handle response message from broker server
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private void OnMessageReceivedHandler()
        {
            while (true)
            {
                try
                {
                    if (_client == null)
                        throw new Exception("Client is closed");

                    byte[] buffer = new byte[_bytesize];
                    _client.GetStream().Read(buffer, 0, _bytesize);
                    var message = Common.GetString(buffer);

                    if (!string.IsNullOrEmpty(message))
                    {
                        OnMessageReceived?.Invoke(JsonConvert.DeserializeObject<Message>(message));
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    SocketException? socketEx = ex.InnerException as SocketException;

                    if(socketEx != null)
                    {
                        ConnectBrokerServer();
                    }    
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}