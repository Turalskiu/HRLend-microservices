namespace AuthorizationApi.Models.DTO.Response.RefreshTokenResponse
{
    public class ListRefreshTokenResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<RefreshTokenResponse> Tokens { get; set; }
    }
}
