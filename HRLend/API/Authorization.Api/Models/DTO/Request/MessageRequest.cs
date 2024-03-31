namespace AuthorizationApi.Models.DTO.Request
{
    public class MessageRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
