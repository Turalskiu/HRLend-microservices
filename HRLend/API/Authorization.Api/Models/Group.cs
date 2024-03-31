using System.Text.Json.Serialization;

namespace AuthorizationApi.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        [JsonIgnore]
        public int TypeId { get; set; }
        public string Title { get; set; }

        public GroupType Type { get; set; }
        public List<User> Users { get; set; }
    }
}
