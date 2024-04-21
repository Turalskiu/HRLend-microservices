namespace Messenger.Api.Hubs.Models.Response
{
    public class SystemMessageResponse
    {
        public SYSTEM_MESSAGE_TYPE Type { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }

        public DeleteMessageInfoResponse? DeleteMessageInfo { get; set; }
        public UpdateChatTitleInfoResponse? UpdateChatTitleInfo { get; set; }
    }

    public class DeleteMessageInfoResponse
    {
        public string MessageGuid { get; set; }
    }

    public class UpdateChatTitleInfoResponse
    {
        public string NewTitle { get; set; }
    }

}
