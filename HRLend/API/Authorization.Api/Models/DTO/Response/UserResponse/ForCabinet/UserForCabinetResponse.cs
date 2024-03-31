using System.Text.Json.Serialization;

namespace AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet
{
    public class UserForCabinetResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Photo { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName {  get; set; }
        public int? Age { get; set; }

        public List<Role> Roles { get; set; }

    }
}
