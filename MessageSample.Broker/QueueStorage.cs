using MessageSample.Share;

namespace MessageSample.Broker
{
    internal class QueueStorage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public readonly StorageTypeEnum StorageType;
        private readonly Queue<Message> _queue;
        public readonly string? Name;

        public QueueStorage(StorageTypeEnum storageType, string? name)
        {
            _queue = new Queue<Message>();
            StorageType = storageType;
            Name = name;
        }

        /// <summary>
        /// Insert message to queue
        /// </summary>
        /// <param name="message"></param>
        public void Enqueue(Message message)
        {
            _queue.Enqueue(message);
        }

        /// <summary>
        /// Dequeue one message
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Message? Dequeue()
        {
            if (!IsEmpty())
                return _queue.Dequeue();
            return null;
        }

        /// <summary>
        /// Dequeue multiple messages
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public IEnumerable<Message> Dequeue(int batchSize = 10)
        {
            for (int i = 0; i < batchSize && _queue.Count > 0; i++)
            {
                yield return _queue.Dequeue();
            }
        }

        public bool IsEmpty()
        {
            return _queue.Count() == 0;
        }
    }
}