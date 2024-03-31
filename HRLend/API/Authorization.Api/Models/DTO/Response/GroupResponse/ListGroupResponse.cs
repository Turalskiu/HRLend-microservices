using AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet;

namespace AuthorizationApi.Models.DTO.Response.GroupResponse
{
    public class ListGroupResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<GroupResponse> Groups { get; set; }
    }
}
