namespace Messenger.Api.Hubs.Models
{
    public class UserMessage
    {
        public string Guid { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
