namespace HRApi.Domain
{
    public class Competence
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        public string Title { get; set; }

        public List<CompetenceAndSkill> Skills { get; set; }
    }
}
