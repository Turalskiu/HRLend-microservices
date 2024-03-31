namespace AuthorizationApi.Models.DTO.Session
{
    public class UpdatePasswordSession
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
        public string Code { get; set; }
    }
}
