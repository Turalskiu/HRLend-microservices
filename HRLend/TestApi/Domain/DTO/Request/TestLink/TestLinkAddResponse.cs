namespace TestApi.Domain.DTO.Request.TestLink
{
    public class TestLinkAddResponse
    {
        public string Title { get; set; }
        public int TestId { get; set; }
        public int TypeId { get; set; }
        public int? CandidateId { get; set; }
        public int? GroupId { get; set; }
        public int? LimitCandidateCount { get; set; }
        public int LimitAttempt { get; set; }
        public DateTime DateExpired { get; set; }
    }
}
