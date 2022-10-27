using MessageSample.Share;

namespace MessageSample.Broker
{
    internal class SubcribeStorageInfo
    {
        public Guid StorageId { get; set; }
        public string? StorageName { get; set; }
        public StorageTypeEnum StorageType { get; set; }
    }
}