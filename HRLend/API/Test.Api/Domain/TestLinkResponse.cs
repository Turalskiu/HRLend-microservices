namespace TestApi.Domain
{
    public class TestLinkResponse
    {
        public int Id { get; set; }
        public int TestLinkId { get; set; }
        public int StatusId { get; set; }
        public int? UserId { get; set; }
        public int NumberAttempt { get; set; }
        public DateTime DateCreate { get; set; }
        public string? TestGeneratedLink { get; set; }

        public User Candidate { get; set; }
        public AnonymousUser AnonymousCandidate { get; set; }

        public TestLinkResponseStatus Status { get; set; }
        public TestResult TestResult { get; set; }
    }
}
