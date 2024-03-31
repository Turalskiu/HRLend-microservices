namespace AuthorizationApi.Models
{
    public class CabinetStatus
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }


    public enum CABINET_STATUS
    {
        ACTIVATED = 1,
        BLOCKED = 2,
        DELETED = 3
    }
}
