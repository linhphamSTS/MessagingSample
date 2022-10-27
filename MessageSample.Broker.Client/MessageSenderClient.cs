using MessageSample.Share;

namespace MessageSample.Broker.Client
{
    public sealed class MessageSenderClient : MassageBrokerClientBase
    {
        private MessageSenderClient(string connectionString)
        {
            ClientType = ClientTypeEnum.Sender;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Initial a queue client connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static async Task<MessageSenderClient> GetClientQueueAsync(string connectionString, string queueName)
        {
            var client = new MessageSenderClient(connectionString)
            {
                _storageType = StorageTypeEnum.Queue,
                _storageName = queueName
            };

            await client.ConnectBrokerServerAsync();
            await client.CreateQueueIfNotExist(queueName);
            return client;
        }

        /// <summary>
        /// Inital a topic client connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public static async Task<MessageSenderClient> GetClientTopicAsync(string connectionString, string topicName)
        {
            var client = new MessageSenderClient(connectionString)
            {
                _storageType = StorageTypeEnum.Topic,
                _storageName = topicName
            };

            await client.ConnectBrokerServerAsync();
            await client.CreateTopicIfNotExist(topicName);
            return client;
        }

        /// <summary>
        /// Send message to broker
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SendMessageAsync(string? message)
        {
            if (_client == null)
                throw new Exception("Cannot connect to broker server");

            var commandActiom = _storageType == StorageTypeEnum.Queue
                 ? $"{CommandActionEnum.InsertQueueMessage}"
                 : $"{CommandActionEnum.InsertTopicMessage}";

            await SendMessageAsync(commandActiom, message);
        }
    }
}