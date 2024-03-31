namespace TestApi.Domain.DTO.Memory.TestPassage
{
    public class LinkInfo
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public int? LimitCandidateCount { get; set; }
        public int LimitAttempt { get; set; }
        public int CandidateCount { get; set; }
        public string Link { get; set; }
        public DateTime DateExpired { get; set; }
    }
}
