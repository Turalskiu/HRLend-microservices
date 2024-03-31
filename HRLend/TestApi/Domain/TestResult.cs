namespace TestApi.Domain
{
    public class TestResult
    {
        public int Id { get; set; }
        public int TestLinkResponseId { get; set; }
        public bool IsPassed { get; set; }
        public string TestResultLink { get; set; }
        public string TestTemplateStatisticsLink { get; set; }

    }
}
