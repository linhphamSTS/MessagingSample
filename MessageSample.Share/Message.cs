namespace MessageSample.Share
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string? Data { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public TimeSpan ExpiredDate { get; set; } = TimeSpan.FromDays(7);

        public static Message Create(string? data)
        {
            return new Message { Data = data };
        }
    }
}