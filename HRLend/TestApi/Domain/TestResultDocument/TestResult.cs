using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace TestApi.Domain.TestResultDocument
{

    public class TestResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        [BsonElement("test_info")]

        public TestInfo TestInfo { get; set; }

        [BsonElement("settings")]
        public SettingsTest Settings { get; set; }

        [BsonElement("user_result")]
        public UserResult UserResult { get; set; }

        [BsonElement("test_module_result")]
        public List<TestModuleResult> TestModuleResult { get; set; }

        [BsonElement("questions")]
        public List<Question> Questions { get; set; }
    }


    public class Option
    {
        [BsonElement("is_true")]
        public bool IsTrue { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("is_user_answer")]
        public bool IsUserAnswer { get; set; }
    }


    public class UserResultQuestion
    {
        [BsonElement("values")]
        public int Values { get; set; }

        [BsonElement("is_skip")]
        public bool IsSkip { get; set; }

        [BsonElement("is_passed")]
        public bool IsPassed { get; set; }
    }


    public class Question
    {
        [BsonElement("test_module_id")]
        public string TestModuleId { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("max_value")]
        public int MaxValue { get; set; }

        [BsonElement("options")]
        public List<Option> Options { get; set; }

        [BsonElement("user_result")]
        public UserResultQuestion UserResult { get; set; }
    }




    public class UserResultTestModule
    {
        [BsonElement("values")]
        public int Values { get; set; }

        [BsonElement("is_passed")]
        public bool IsPassed { get; set; }

    }


    public class TestModuleResult
    {
        [BsonElement("test_module_id")]
        public string TestModuleId { get; set; }

        [BsonElement("min_value_for_passed")]
        public int MinValueForPassed { get; set; }
        [BsonElement("max_value")]
        public int MaxValue { get; set; }

        [BsonElement("user_result")]
        public UserResultTestModule UserResult { get; set; }
    }


    public class UserResult
    {
        [BsonElement("duration_in_seconds")]
        public int DurationInSeconds { get; set; }

        [BsonElement("values")]
        public int Values { get; set; }

        [BsonElement("is_completed")]
        public bool IsCompleted { get; set; }

        [BsonElement("is_passed")]
        public bool IsPassed { get; set; }

        [BsonElement("date_start")]
        public DateTime? DateStart { get; set; }

        [BsonElement("date_end")]
        public DateTime? DateEnd { get; set; }
    }


    public class SettingsTest
    {
        [BsonElement("limit_duration_in_seconds")]
        public int LimitDurationInSeconds { get; set; }

        [BsonElement("is_timer")]
        public bool IsTimer { get; set; }

        [BsonElement("is_corrected")]
        public bool IsCorrected { get; set; }
    }

    public class TestInfo
    {
        [BsonElement("count_questions")]
        public int CountQuestions { get; set; }

        [BsonElement("max_value")]
        public int MaxValue { get; set; }
    }
}
