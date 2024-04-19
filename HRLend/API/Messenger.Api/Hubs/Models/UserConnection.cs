namespace Messenger.Api.Hubs.Models
{
    public class UserConnection
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? UserPhoto { get; set; }
        public string ChatLink { get; set; }
    }
}
