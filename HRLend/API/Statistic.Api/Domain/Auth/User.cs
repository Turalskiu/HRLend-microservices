namespace StatisticApi.Domain.Auth
{
    public class User
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        public IEnumerable<int> Roles { get; set; }
    }

    public enum ROLE
    {
        USER = 1,
        ADMIN = 2,
        CABINET_ADMIN = 3,
        CABINET_HR = 4,
        CABINET_EMPLOYEE = 5,
        CABINET_CANDIDATE = 6
    }
}
