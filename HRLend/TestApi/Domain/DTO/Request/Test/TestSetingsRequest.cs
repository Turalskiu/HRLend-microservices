namespace TestApi.Domain.DTO.Request.Test
{
    public class TestSetingsRequest
    {
        public bool MixGuestions { get; set; }
        public bool MixAnswers { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsTimer { get; set; }
        public bool IsShowRecommendMaterials { get; set; }
        public bool IsShowResult { get; set; }
        public bool IsShowResultAndTrueQuestions { get; set; }
    }
}
