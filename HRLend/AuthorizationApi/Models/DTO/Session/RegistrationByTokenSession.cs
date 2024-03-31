namespace AuthorizationApi.Models.DTO.Session
{
    public class RegistrationByTokenSession
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int CabinetId { get; set; }
        public int CabinetRoleId { get; set; }
        public string KeyActivate { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
