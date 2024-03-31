namespace AuthorizationApi.Models.DTO.Response
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public List<Role> Roles { get; set; }

        public string JwtToken { get; set; }
    }
}
