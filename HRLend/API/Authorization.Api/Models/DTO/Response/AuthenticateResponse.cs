using System.Text.Json.Serialization;

namespace AuthorizationApi.Models.DTO.Response
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public DateTime Created { get; set; }
        public List<Role> Roles { get; set; }

        public bool IsBlocked { get; set; } = false;
        public DateTime? DateBlocked { get; set; }
        public DateTime? DateUnblocked { get; set; }
        public string? ReasonBlocked { get; set; }

        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse() { }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            CabinetId = user.CabinetId;
            Username = user.Username;
            Email = user.Email;
            Roles = user.Roles;
            Created = user.DateCreate;
            Image = user.Photo;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
