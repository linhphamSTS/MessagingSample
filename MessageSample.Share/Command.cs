namespace MessageSample.Share
{
    public enum CommandActionEnum
    {
        CreateQueue, 
        DeleteQueue,
        InsertQueueMessage,

        CreateTopic,
        DeleteTopic,
        InsertTopicMessage
    }

    public class Command
    {
        public string? Issuer { get; set; }
        public CommandActionEnum? Action { get; set; }
        public StorageTypeEnum StorageType { get; set; }
        public string? StorageName { get; set; }
        public string? Data { get; set; }
    }
}