using MessageSample.Share;

namespace MessageSample.Broker
{
    internal interface IMessageBroker
    {
        Distributor Distributor { get; }

        void AcceptClient(ClientSession client);

        void AcceptMessage(string message);

        void AddSubscriber(ClientSession clientSession);

        void RemoveSubscriber(ClientSession clientSession);
    }

    /// <summary>
    /// Handler distributor and subscriber
    /// </summary>
    internal class MessageBroker : IMessageBroker
    {
        public Distributor Distributor { get; }

        public MessageBroker()
        {
            Distributor = new Distributor();
            Distributor.StartDistribute();
        }

        public void AcceptClient(ClientSession client)
        {
            client.VisitBroker(this);
            client.StartClient();
        }

        public void AddSubscriber(ClientSession clientSession)
        {
            if (clientSession.ClientType == ClientTypeEnum.Sender)
            {
                if (!Distributor.Senders.Any(x => x.ClientEndpoint == clientSession.ClientEndpoint))
                {
                    Distributor.Senders.Add(clientSession);
                }
            }
            else
            {
                if (!Distributor.Receivers.Any(x => x.ClientEndpoint == clientSession.ClientEndpoint))
                    Distributor.Receivers.Add(clientSession);
            }

            if (clientSession.SubcribeStorage != null)
            {
                var storage = Distributor.Storage.FindStorage(clientSession.SubcribeStorage.StorageName, clientSession.SubcribeStorage.StorageType);
                if (storage != null && clientSession.SubcribeStorage.StorageId != storage.Id)
                    clientSession.SubcribeStorage.StorageId = storage.Id;
            }
        }

        public void RemoveSubscriber(ClientSession clientSession)
        {
            if (clientSession.ClientType == ClientTypeEnum.Sender)
            {
                var sender = Distributor.Senders.FirstOrDefault(x => x.ClientEndpoint == clientSession.ClientEndpoint);
                if (sender != null)
                    Distributor.Senders.Remove(sender);
            }
            else
            {
                var receiver = Distributor.Receivers.FirstOrDefault(x => x.ClientEndpoint == clientSession.ClientEndpoint);
                if (receiver != null)
                    Distributor.Receivers.Remove(receiver);
            }
        }

        public void AcceptMessage(string message)
        {
            var command = CommandParser(message);
            ExecuteCommand(command);
        }

        private void ExecuteCommand(Command? command)
        {
            switch (command?.Action)
            {
                case CommandActionEnum.CreateQueue:
                    Distributor.Storage.InitStorage(command?.Data, StorageTypeEnum.Queue);
                    break;

                case CommandActionEnum.DeleteQueue:
                    Distributor.Storage.DeleteStorage(command?.Data, StorageTypeEnum.Queue);
                    break;

                case CommandActionEnum.InsertQueueMessage:
                    Distributor.Storage.InsertMessage(command.StorageName, command.Data, StorageTypeEnum.Queue);
                    break;

                case CommandActionEnum.CreateTopic:
                    Distributor.Storage.InitStorage(command?.Data, StorageTypeEnum.Topic);
                    break;

                case CommandActionEnum.DeleteTopic:
                    Distributor.Storage.DeleteStorage(command?.Data, StorageTypeEnum.Topic);
                    break;

                case CommandActionEnum.InsertTopicMessage:
                    Distributor.Storage.InsertMessage(command.StorageName, command.Data, StorageTypeEnum.Topic);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private static Command? CommandParser(string? message)
        {
            if (string.IsNullOrEmpty(message))
                throw new Exception("message cannot empty");

            var info = message.Split('|');

            return new Command
            {
                Issuer = info[0],
                Action = Enum.Parse<CommandActionEnum>(info[1]),
                StorageType = Enum.Parse<StorageTypeEnum>(info[2]),
                StorageName = info[3],
                Data = info[4]
            };
        }
    }
}