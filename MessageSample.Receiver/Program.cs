using MessageSample.Broker.Client;
using MessageSample.Share;

Console.WriteLine("App Receiver");
var client = await MessageReceiverClient.GetClientQueueAsync("127.0.0.1:8100", "QueueDemo");

client.OnMessageReceived = (Message? e) =>
{
    Console.WriteLine($"Received Message: {e?.Data} at {DateTime.UtcNow}");
};