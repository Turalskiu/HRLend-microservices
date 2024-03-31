using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestApi.Domain.TestTemplateDocument
{
    public class TestTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("settings")]
        public Settings Settings { get; set; }

        [BsonElement("result_setings")]
        public ResultSettings ResultSettings { get; set; }

        [BsonElement("competencies")]
        public List<Competency> Competencies { get; set; }
    }


    public class Skill
    {

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("required_code")]
        public int RequiredCode { get; set; }

        [BsonElement("id_test_module")]
        public string IdTestModule { get; set; }
    }


    public class Competency
    {

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("required_code")]
        public int RequiredCode { get; set; }

        [BsonElement("skills")]
        public List<Skill> Skills { get; set; }
    }


    public class Settings
    {
        [BsonElement("mix_questions")]
        public bool MixQuestions { get; set; }

        [BsonElement("mix_answers")]
        public bool MixAnswers { get; set; }

        [BsonElement("is_correct")]
        public bool IsCorrect { get; set; }

        [BsonElement("is_timer")]
        public bool IsTimer { get; set; }
    }


    public class ResultSettings
    {
        [BsonElement("is_show_recommend_materials")]
        public bool IsShowRecommendMaterials { get; set; }

        [BsonElement("is_show_result_and_true_questions")]
        public bool IsShowResultAndTrueQuestions { get; set; }

        [BsonElement("is_show_result")]
        public bool IsShowResult { get; set; }
    }
}
