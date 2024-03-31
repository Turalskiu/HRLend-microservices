using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TestApi.Domain.TestTemplateStatisticsDocument
{
    public class TemplateStatistics
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("is_passed")]
        public bool IsPassed { get; set; }

        [BsonElement("percent")]
        public double Percent { get; set; }

        [BsonElement("competencies")]
        public List<Competency> Competencies { get; set; }
    }


    public class Skill
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("required_code")]
        public int RequiredCode { get; set; }

        [BsonElement("is_passed")]
        public bool IsPassed { get; set; }

        [BsonElement("percent")]
        public double Percent { get; set; }

        [BsonElement("id_test_module")]
        public string IdTestModule { get; set; }
    }

    public class Competency
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("required_code")]
        public int RequiredCode { get; set; }

        [BsonElement("is_passed")]
        public bool IsPassed { get; set; }

        [BsonElement("percent")]
        public double Percent { get; set; }

        [BsonElement("skills")]
        public List<Skill> Skills { get; set; }
    }
}
