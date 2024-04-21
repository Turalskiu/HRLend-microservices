
namespace Messenger.Api.Hubs.Models.Request
{
    public class SystemMessageRequest
    {
        public SYSTEM_MESSAGE_TYPE Type { get; set; }
        public DeleteMessageInfoRequest? DeleteMessageInfo { get; set; }
        public DeleteUserInfoRequest? DeleteUserInfo { get; set; }
        public AddUserInfoRequest? AddUserInfo { get; set; }
        public UpdateChatTitleInfoRequest? UpdateChatTitleInfo { get; set; }
    }

    public class DeleteMessageInfoRequest
    {
        public int CreatorId { get; set; }
        public string MessageGuid { get; set; }
    }

    public class DeleteUserInfoRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }

    public class AddUserInfoRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Photo {  get; set; }
    }


    public class UpdateChatTitleInfoRequest
    {
        public string Title { get; set; }
        public string NewTitle { get; set; }
    }

    //public enum SYSTEM_MESSAGE_TYPE
    //{
    //    DELETE_CHAT = 1,
    //    DELETE_MESSAGE = 2,
    //    DELETE_USER = 3,
    //    ADD_USER = 4,
    //    EXIT_USER = 5,
    //    UPDATE_CHAT_TITLE = 6
    //}
}
