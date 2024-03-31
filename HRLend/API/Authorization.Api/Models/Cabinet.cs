using System.Text.Json.Serialization;

namespace AuthorizationApi.Models
{
    public class Cabinet
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int StatusId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }

        public CabinetStatus Status {get; set;}

        public List<User> Users { get; set; }
        public List<Group> Groups { get; set; }
    }
}
