using MessageSample.Share;
using System.Collections.Concurrent;

namespace MessageSample.Broker
{
    internal class StorageManagement
    {
        public ConcurrentQueue<QueueStorage> QueueStorages { get; } //safety for multiple clients accesing

        public StorageManagement()
        {
            QueueStorages = new ConcurrentQueue<QueueStorage>();
        }

        public void InitStorage(string? name, StorageTypeEnum storageType)
        {
            var storageItem = FindStorage(name, storageType);

            if (storageItem == null)
            {
                QueueStorages.Enqueue(new QueueStorage(storageType, name));
            }
        }

        public void DeleteStorage(string? name, StorageTypeEnum storageType)
        {
            var storageItem = FindStorage(name, storageType);

            if (storageItem != null)
            {
                QueueStorages.TryDequeue(out storageItem);
            }
        }

        public QueueStorage? FindStorage(string? name, StorageTypeEnum storageType)
        {
            return QueueStorages
                .FirstOrDefault(c => string.Compare(name, c.Name, true) == 0 && c.StorageType == storageType);
        }

        public void InsertMessage(string? name, string? data, StorageTypeEnum storageType)
        {
            var storageItem = FindStorage(name, storageType);

            if (storageItem != null)
            {
                storageItem.Enqueue(Message.Create(data));
            }
        }
    }
}