using System.Text.Json.Serialization;

namespace AuthorizationApi.Models.DTO.Response.RegistrationTokenResponse
{
    public class RegistrationTokenResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public int CabinetRole { get; set; }
    }
}
