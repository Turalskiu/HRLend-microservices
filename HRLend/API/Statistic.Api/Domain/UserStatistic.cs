using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace StatisticApi.Domain
{
    public class User
    {
        [BsonElement("user_id")]
        public int UserId { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }
    }

    public class Competency
    {
        [BsonElement("test_id")]
        public int TestId { get; set; }
        [BsonElement("test_title")]
        public string TestTitle { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("date_create")]
        public DateTime DateCreate { get; set; }

        [BsonElement("percent")]
        public double Percent { get; set; }
    }

    public class Skill
    {
        [BsonElement("test_id")]
        public int TestId { get; set; }
        [BsonElement("test_title")]
        public string TestTitle { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("date_create")]
        public DateTime DateCreate { get; set; }

        [BsonElement("percent")]
        public double Percent { get; set; }
    }

    public class UserStatistic
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user")]
        public User User { get; set; }

        [BsonElement("competencies")]
        public List<Competency> Competencies { get; set; }

        [BsonElement("skills")]
        public List<Skill> Skills { get; set; }
    }
}
