namespace AuthorizationApi.Models.QueueMessage
{
    public class GroupQM
    {
        public int MessageType { get; set; }
        public int GroupId { get; set; }
        public string? GroupTitle { get; set; }
    }

    public enum GROUP_MESSAGE_TYPE
    {
        ADD = 1,
        DELETE = 2,
        UPDATE_TITLE = 3
    }
}
