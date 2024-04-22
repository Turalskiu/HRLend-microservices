namespace Contracts.Test.Queue
{

    public class UserStatisticQM
    {
        public int MessageType { get; set; }
        public UserQM User {  get; set; }
        public HashSet<CompetencyQM> Competencies { get; set; }
        public HashSet<SkillQM> Skills { get; set; }

    }


    public class UserQM
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class CompetencyQM
    {
        public int TestId { get; set; }
        public string TestTitle { get; set; }
        public string Title { get; set; }
        public DateTime DateCreate { get; set; }
        public double Percent { get; set; }
    }

    public class SkillQM
    {
        public int TestId { get; set; }
        public string TestTitle { get; set; }
        public string Title { get; set; }
        public DateTime DateCreate { get; set; }
        public double Percent { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SkillQM other = (SkillQM)obj;
            return this.Title.Equals(other.Title);
        }

        public override int GetHashCode()
        {
            unchecked // чтобы избежать переполнения целочисленного значения
            {
                int hash = this.Title.Length; 
                foreach (char c in this.Title)
                {
                    hash = hash * 23 + c.GetHashCode(); 
                }
                return hash;
            }
        }
    }


    public enum USER_STATISTIC_TYPE
    {
        TEST = 1
    }
}
