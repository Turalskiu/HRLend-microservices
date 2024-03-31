using AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet;


namespace AuthorizationApi.Models.DTO.Response.CabinetResponse
{
    public class CabinetResponse
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }

        public CabinetStatus Status { get; set; }
        public IEnumerable<UserShortForCabinetResponse> Users { get; set; }
        public IEnumerable<AuthorizationApi.Models.DTO.Response.GroupResponse.GroupResponse> Groups { get; set; }
    }
}
