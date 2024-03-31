namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet
{
    public class ListUserForCabinetResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<UserShortAndRoleForCabinetResponse> Users { get; set; }
    }
}
