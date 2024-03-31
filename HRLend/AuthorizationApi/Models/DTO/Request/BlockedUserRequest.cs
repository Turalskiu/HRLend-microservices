namespace AuthorizationApi.Models.DTO.Request
{
    public class BlockedUserRequest
    {
        public int UserId { get; set; }
        public string? ReasonBlocked { get; set; }
        public DateTime DateUnblocked { get; set; }
    }
}
