namespace AuthorizationApi.Models.DTO.Session
{
    public class UserSession
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        public IEnumerable<int> Roles { get; set; }
    }
}
