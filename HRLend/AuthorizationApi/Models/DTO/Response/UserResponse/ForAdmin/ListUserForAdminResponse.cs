namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForAdmin
{
    public class ListUserForAdminResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<UserShortForAdminResponse> Users { get; set; }
    }
}
