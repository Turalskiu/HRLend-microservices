namespace AuthorizationApi.Models.DTO.Response.UserResponse
{
    public class BlockedUserResponse
    {
        public DateTime DateBlocked { get; set; }
        public DateTime DateUnblocked { get; set; }
        public string? ReasonBlocked { get; set; }
    }
}
