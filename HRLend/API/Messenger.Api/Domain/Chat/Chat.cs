using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Messenger.Api.Domain.Chat
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("creator")]
        public Creator Creator { get; set; }

        [BsonElement("users")]
        public List<User> Users { get; set; }

        [BsonElement("messages")]
        public List<Message> Messages { get; set; }
    }

    public class Creator
    {
        [BsonElement("creator_id")]
        public int CreatorId { get; set; }
    }

    public class User
    {
        [BsonElement("user_id")]
        public int UserId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("photo")]
        public string Photo { get; set; }
    }

    public class Message
    {
        [BsonElement("guid")]
        public string Guid { get; set; }
        [BsonElement("is_system")]
        public bool IsSystem{ get; set; }
        [BsonElement("user")]
        public User User { get; set; }

        [BsonElement("message")]
        public string Text { get; set; }

        [BsonElement("date_create")]
        public DateTime DateCreated { get; set; }
    }
}
