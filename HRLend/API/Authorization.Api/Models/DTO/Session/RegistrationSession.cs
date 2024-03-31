namespace AuthorizationApi.Models.DTO.Session
{
    public class RegistrationSession
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CabinetTitle { get; set; }
        public string KeyActivate { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
