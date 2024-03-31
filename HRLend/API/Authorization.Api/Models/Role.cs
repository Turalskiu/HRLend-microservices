using System.Text.Json.Serialization;

namespace AuthorizationApi.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Title { get; set; }

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
