namespace AuthorizationApi.Models.DTO.Response.RegistrationTokenResponse
{
    public class ListRegistrationTokenResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<RegistrationTokenResponse> Tokens { get; set; }
    }
}
