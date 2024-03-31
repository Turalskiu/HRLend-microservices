using TestApi.Domain.DTO.Response.AnonymousUser;
using TestApi.Domain.DTO.Response.TestResult;

namespace TestApi.Domain.DTO.Response.TestLinkResponse
{
    public class TestLinkResponseResponse
    {
        public int Id { get; set; }
        public User? Candidate { get; set; }
        public AnonymousUserResponse? AnonymousCandidate { get; set; }
        public int NumberAttempt { get; set; }
        public DateTime DateCreate { get; set; }
        public string TestGeneratedLink { get; set; }

        public TestLinkResponseStatus Status { get; set; }
        public TestResultResponse? TestResult { get; set; }
    }
}
