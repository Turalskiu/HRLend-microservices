namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet
{
    public class UserShortAndRoleForCabinetResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public List<Role> Roles { get; set; }
    }
}
