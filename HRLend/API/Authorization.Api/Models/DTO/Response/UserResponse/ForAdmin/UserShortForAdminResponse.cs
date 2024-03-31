namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForAdmin
{
    public class UserShortForAdminResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public List<Role> Roles { get; set; }
        public UserStatus Status { get; set; }
    }
}
