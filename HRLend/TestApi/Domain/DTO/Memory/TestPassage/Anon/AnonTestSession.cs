namespace TestApi.Domain.DTO.Memory.TestPassage.Anon
{
    public class AnonTestSession
    {
        public int ResponseId { get; set; }
        public int? AnonUserId { get; set; }
        public DateTime DateCreate { get; set; }
        public TestInfo Test { get; set; }
        public LinkInfo Link { get; set; }
    }
}
