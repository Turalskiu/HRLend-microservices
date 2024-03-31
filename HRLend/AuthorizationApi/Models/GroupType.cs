namespace AuthorizationApi.Models
{
    public class GroupType
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public enum GROUP_TYPE
    {
        CANDIDATE = 1,
        EMPLOYEE = 2
    }
}
