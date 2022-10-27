using MessageSample.Broker.Client;

Console.WriteLine("App Sender");
var client = await MessageSenderClient.GetClientQueueAsync("127.0.0.1:8100", "QueueDemo");
while (true)
{
    Console.Write("Please input message: ");
    string? message = Console.ReadLine();
    await client.SendMessageAsync(message);
}