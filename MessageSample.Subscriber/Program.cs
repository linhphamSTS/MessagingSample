using MessageSample.Broker.Client;
using MessageSample.Share;

Console.WriteLine("App Subscriber 1");
var client = await MessageReceiverClient.GetClientTopicAsync("127.0.0.1:8100", "TopicDemo");

client.OnMessageReceived = (Message? e) =>
{
    Console.WriteLine($"Received Message: {e?.Data} at {DateTime.UtcNow}");
};