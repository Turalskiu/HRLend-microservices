namespace AuthorizationApi.Models.QueueMessage
{
    public class UserQM
    {
        public int MessageType { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhoto { get; set; }
    }

    public enum USER_MESSAGE_TYPE{
        ADD = 1,
        UPDATE_USERNAME = 2,
        UPDATE_PHOTO = 3
    }
}
