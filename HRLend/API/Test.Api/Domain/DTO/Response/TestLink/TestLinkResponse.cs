using TestApi.Domain.DTO.Response.AnonymousUser;

namespace TestApi.Domain.DTO.Response.TestLink
{
    public class TestLinkResponse
    {
        public int Id { get; set; }
        public User? Candidate { get; set; }
        public Group? Group { get; set; }
        public int? LimitCandidateCount { get; set; }
        public int LimitAttempt {  get; set; }
        public int CandidateCount { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateExpired { get; set; }
        public DateTime? DateClosed { get; set; }
        public TestLinkStatus Status { get; set; }
        public TestLinkType Type { get; set; }
    }
}
