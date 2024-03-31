using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthorizationApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        [JsonIgnore]
        public int StatusId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }
        public DateTime? DateActivation { get; set; }
        public DateTime? DateBlocked { get; set; }
        public DateTime? DateUnblocked { get; set; }
        public string? ReasonBlocked { get; set; }

        public UserInfo Info { get; set; }

        public UserStatus Status { get; set; }

        public List<Role> Roles { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
