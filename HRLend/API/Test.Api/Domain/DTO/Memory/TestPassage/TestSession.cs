namespace TestApi.Domain.DTO.Memory.TestPassage
{
    public class TestSession
    {
        public int ResponseId { get; set; }
        public int NumberAttempt { get; set; }
        public DateTime DateCreate { get; set; }
        public TestInfo Test {  get; set; }
        public LinkInfo Link { get; set; }
        public CandidateInfo Candidate { get; set; }
    }
}
