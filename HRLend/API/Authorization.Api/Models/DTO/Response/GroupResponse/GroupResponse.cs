using System.Text.Json.Serialization;

namespace AuthorizationApi.Models.DTO.Response.GroupResponse
{
    public class GroupResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public GroupType Type { get; set; }
    }
}
