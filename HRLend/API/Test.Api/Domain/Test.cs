namespace TestApi.Domain
{
    public class Test
    {
        public int Id { get; set; }
        public int HrId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TestTemplateLink { get; set; }

        public IList<TestLink> Links { get; set; }
    }
}
