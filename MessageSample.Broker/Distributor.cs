using MessageSample.Share;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace MessageSample.Broker
{
    /// <summary>
    /// Responsible for message delivery
    /// </summary>
    internal class Distributor
    {
        private const int MAX_PROCESS_ITEMS = 100;
        public List<ClientSession> Senders { get; } = new List<ClientSession>();

        public List<ClientSession> Receivers { get; } = new List<ClientSession>();

        public StorageManagement Storage { get; } = new StorageManagement();

        /// <summary>
        /// Run message distribution in background thread
        /// </summary>
        public void StartDistribute()
        {
            Thread thDistributeQueues = new(DistributeQueues);
            Thread thDistributeTopic = new(DistributeTopics);
            thDistributeTopic.Start();
            thDistributeQueues.Start();
        }

        /// <summary>
        /// Distribute for Queues
        /// </summary>
        private void DistributeQueues()
        {
            while (true)
            {
                var queues = Storage.QueueStorages.Where(x => x.StorageType == StorageTypeEnum.Queue);
                if (queues.Any())
                {
                    Parallel.ForEach(queues, new ParallelOptions { MaxDegreeOfParallelism = MAX_PROCESS_ITEMS }, (queue) =>
                    {
                        ProcessQueue(queue);
                    });
                }
            }
        }

        /// <summary>
        /// Distribute for Topics
        /// </summary>
        private void DistributeTopics()
        {
            while (true)
            {
                var topics = Storage.QueueStorages.Where(x => x.StorageType == StorageTypeEnum.Topic);
                if (topics.Any())
                {
                    Parallel.ForEach(topics, new ParallelOptions { MaxDegreeOfParallelism = MAX_PROCESS_ITEMS }, (topic) =>
                    {
                        ProcessTopic(topic);
                    });
                }
            }
        }

        /// <summary>
        /// Process send message to receiver
        /// </summary>
        /// <param name="queue"></param>
        private void ProcessQueue(QueueStorage queue)
        {
            //queue only send to one comsumer which connected first
            var receiver = Receivers.Where(x => x.SubcribeStorage?.StorageId == queue.Id)
                                .OrderBy(x => x.ConnectedDate).FirstOrDefault();

            if (receiver != null && receiver.Client != null)
            {
                NetworkStream networkStream = receiver.Client.GetStream();
                while (!queue.IsEmpty())
                {
                    var queueItem = queue.Dequeue();
                    SendMessage(networkStream, queueItem);
                }
            }
        }

        /// <summary>
        /// Process send message to subcribers
        /// </summary>
        /// <param name="topic"></param>
        private void ProcessTopic(QueueStorage topic)
        {
            var receivers = Receivers.Where(x => x.SubcribeStorage?.StorageId == topic.Id)
                                .OrderBy(x => x.ConnectedDate);

            if (receivers.Any())
            {
                while (!topic.IsEmpty())
                {
                    var queueItem = topic.Dequeue();

                    Parallel.ForEach(receivers, new ParallelOptions { MaxDegreeOfParallelism = MAX_PROCESS_ITEMS }, (receiver) =>
                    {
                        if (receiver != null && receiver.Client != null)
                        {
                            NetworkStream networkStream = receiver.Client.GetStream();
                            SendMessage(networkStream, queueItem);
                        }
                    });
                }
            }
        }

        private static void SendMessage(NetworkStream networkStream, Message? queueItem)
        {
            var message = JsonConvert.SerializeObject(queueItem);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            networkStream.Write(bytes, 0, bytes.Length);
        }
    }
}