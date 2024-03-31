
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KnowledgeBaseApi.Domain
{
    public class Profession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("competencies")]
        public List<Competence> Competencies { get; set; }
    }


    public class Competence
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("competence_need")]
        public CompetenceNeed CompetenceNeed { get; set; }

        [BsonElement("skills")]
        public List<Skill> Skills { get; set; }
    }

    public class CompetenceNeed
    {
        [BsonElement("required_code")]
        public int RequiredCode { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }
    }

    public class Skill
    {
        [BsonElement("skill_need")]
        public SkillNeed SkillNeed { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("test_module_id")]
        public string TestModuleId { get; set; }
    }

    public class SkillNeed
    {
        [BsonElement("required_code")]
        public int RequiredCode { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }
    }


    public enum SKILL_NEED
    {
        REQUIRE_HARD = 1,
        REQUIRE_MIDDLE = 2,
        REQUIRE_SOFT = 3
    }

    public enum COMPETENCE_NEED
    {
        REQUIRE_HARD = 1,
        REQUIRE_MIDDLE = 2,
        REQUIRE_SOFT = 3
    }
}
