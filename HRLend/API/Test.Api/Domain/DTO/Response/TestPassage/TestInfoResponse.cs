namespace TestApi.Domain.DTO.Response.TestPassage
{
    public class TestInfoResponse
    {
        public string TestTitle { get; set; }
        public string? TestDescription { get; set; }
        public int LimitAttempt { get; set; }
        public int NumberAttempt { get; set; }
    }
}
