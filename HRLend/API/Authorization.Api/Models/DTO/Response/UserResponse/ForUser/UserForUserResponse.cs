using System.Text.Json.Serialization;

namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForUser
{
    public class UserForUserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }
        public DateTime? DateActivation { get; set; }
        public DateTime? DateBlocked { get; set; }
        public DateTime? DateUnblocked { get; set; }
        public string? ReasonBlocked { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName {  get; set; }
        public int? Age { get; set; }

        public UserStatus Status { get; set; }
        public List<Role> Roles { get; set; }

    }
}
