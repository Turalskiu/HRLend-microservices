namespace TestApi.Domain.DTO.Response.Test
{
    public class ListTestResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<TestShortResponse> Tests{ get; set; }
    }
}
