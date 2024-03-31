using static System.Net.Mime.MediaTypeNames;

namespace TestApi.Domain
{
    public class TestLink
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public int? UserId { get; set; }
        public int? GroupId { get; set; }
        public int? LimitCandidateCount { get; set; }
        public int LimitAttempt { get; set; }
        public int CandidateCount { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateExpired { get; set; }
        public DateTime? DateClosed { get; set; }

        public User Candidate { get; set; }
        public Group Group { get; set; }

        public TestLinkStatus Status { get; set; }
        public TestLinkType Type { get; set; }
        public IEnumerable<TestLinkResponse> Responses { get; set; }
    }
}
