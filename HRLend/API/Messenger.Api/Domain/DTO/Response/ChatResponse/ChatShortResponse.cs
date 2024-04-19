using MongoDB.Bson.Serialization.Attributes;

namespace Messenger.Api.Domain.DTO.Response.ChatResponse
{
    public class ChatShortResponse
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }
}
