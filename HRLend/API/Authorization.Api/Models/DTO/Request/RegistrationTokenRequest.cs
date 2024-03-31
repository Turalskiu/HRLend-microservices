using System.Text.Json.Serialization;

namespace AuthorizationApi.Models.DTO.Request
{
    public class RegistrationTokenRequest
    {
        public DateTime Expires { get; set; }
        public int CabinetRole { get; set; }
    }
}
