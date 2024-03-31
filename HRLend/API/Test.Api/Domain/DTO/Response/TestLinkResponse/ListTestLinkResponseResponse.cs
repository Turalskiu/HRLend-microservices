using TestApi.Domain.DTO.Response.TestLink;

namespace TestApi.Domain.DTO.Response.TestLinkResponse
{
    public class ListTestLinkResponseResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<TestLinkResponseShortResponse> Responses { get; set; }
    }
}
