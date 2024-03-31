using System.Text.Json.Serialization;

namespace AuthorizationApi.Models
{
    public class RegistrationToken
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public int Cabinet { get; set; }
        public int CabinetRole { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
    }
}
