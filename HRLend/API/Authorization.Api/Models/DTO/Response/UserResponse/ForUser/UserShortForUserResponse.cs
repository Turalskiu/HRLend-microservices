namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForUser
{
    public class UserShortForUserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public List<Role> Roles { get; set; }
    }
}
