namespace AuthorizationApi.Models.DTO.Request
{
    public class RestorePasswordRequest
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }

    }
}
