using Messenger.Api.Domain.Chat;

namespace Messenger.Api.Domain.DTO.Request
{
    public class ChatCreateRequest
    {
        public string Title { get; set; }
        public List<User> Users { get; set; }
    }
}
