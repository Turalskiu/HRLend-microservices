namespace HRApi.Domain
{
    public class CompetenceAndSkill
    {
        public int CompetenceId { get; set; }
        public int SkillId { get; set; }
        public int SkillNeedId { get; set; }

        public Competence Competence { get; set; }
        public Skill Skill { get; set; }
        public SkillNeed SkillNeed { get; set; }
    }
}
