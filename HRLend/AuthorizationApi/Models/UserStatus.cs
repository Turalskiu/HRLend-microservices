namespace AuthorizationApi.Models
{
    public class UserStatus
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public enum USER_STATUS
    {
        ACTIVATED = 1,
        BLOCKED = 2,
        DELETED = 3
    }
}
