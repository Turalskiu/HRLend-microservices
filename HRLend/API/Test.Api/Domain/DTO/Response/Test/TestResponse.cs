using TestApi.Domain.DTO.Response.TestLink;

namespace TestApi.Domain.DTO.Response.Test
{
    public class TestResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TestTemplateLink { get; set; }

        public IList<TestLinkShortResponse> Links { get; set; }
    }
}
