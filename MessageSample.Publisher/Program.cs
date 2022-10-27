using MessageSample.Broker.Client;

Console.WriteLine("App Publisher");
var client = await MessageSenderClient.GetClientTopicAsync("127.0.0.1:8100", "TopicDemo");
while (true)
{
    Console.Write("Please input message: ");

    string? message = Console.ReadLine();
    await client.SendMessageAsync(message);
}