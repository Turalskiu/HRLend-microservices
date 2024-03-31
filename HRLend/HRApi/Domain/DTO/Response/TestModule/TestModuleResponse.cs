namespace HRApi.Domain.DTO.Response.TestModule
{
    public class TestModuleResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public OptionsResponse Options { get; set; }
        public RuleResponse Rule { get; set; }
        public List<QuestionResponse> Questions { get; set; }
        public List<RecommendationResponse> Recommendations { get; set; }
    }

    public class RecommendationResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }

    public class OptionsResponse
    {
        public bool IsDefault { get; set; }
        public int CountQuestions { get; set; }
        public int TakeQuestions { get; set; }
        public int LimitDurationInSeconds { get; set; }
    }

    public class RuleResponse
    {
        public int MinValueForPassed { get; set; }
    }

    public class QuestionResponse
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int MaxValue { get; set; }
        public List<OptionResponse> Options { get; set; }
    }

    public class OptionResponse
    {
        public string Id { get; set; }
        public bool IsTrue { get; set; }
        public string Text { get; set; }
    }
}
