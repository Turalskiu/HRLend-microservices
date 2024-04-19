namespace Messenger.Api.Hubs.Models
{
    public class SystemMessage
    {
        public SYSTEM_MESSAGE_TYPE Type { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public enum SYSTEM_MESSAGE_TYPE
    {
        DELETE_CHAT = 1,
        DELETE_MESSAGE = 2,
        DELETE_USER = 3,
        ADD_USER = 4,
        EXIT_USER = 5,
        UPDATE_CHAT_TITLE = 6
    }
}
