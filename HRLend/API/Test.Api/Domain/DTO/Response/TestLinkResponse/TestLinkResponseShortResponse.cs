using TestApi.Domain.DTO.Response.AnonymousUser;
using TestApi.Domain.DTO.Response.TestResult;

namespace TestApi.Domain.DTO.Response.TestLinkResponse
{
    public class TestLinkResponseShortResponse
    {
        public int Id { get; set; }
        public DateTime DateCreate { get; set; }
        public TestLinkResponseStatus Status { get; set; }
        public int NumberAttempt { get; set; }
        public User? Candidate { get; set; }
        public AnonymousUserShortResponse? AnonymousCandidate { get; set; }

        public TestResultResponse? TestResult { get; set; }

    }
}
