namespace Messenger.Api.Hubs.Models.Response
{
    public class UserMessageResponse
    {
        public string Guid { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
