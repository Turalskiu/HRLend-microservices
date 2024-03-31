using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace StatisticApi.Domain.QueueMessage.Statistic
{

    public class UserStatisticQM
    {
        public int MessageType { get; set; }
        public UserQM User {  get; set; }
        public List<CompetencyQM> Competencies { get; set; }
        public List<SkillQM> Skills { get; set; }

    }


    public class UserQM
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class CompetencyQM
    {
        public string Title { get; set; }
        public DateTime DateCreate { get; set; }
        public double Percent { get; set; }
    }

    public class SkillQM
    {
        public string Title { get; set; }
        public DateTime DateCreate { get; set; }
        public double Percent { get; set; }
    }


    public enum USER_STATISTIC_TYPE
    {
        TEST = 1
    }
}
