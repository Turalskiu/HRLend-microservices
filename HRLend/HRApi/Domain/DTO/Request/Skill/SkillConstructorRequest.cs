namespace HRApi.Domain.DTO.Request.Skill
{
    public class SkillConstructorRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SkillNeed { get; set; }
        public bool IsUpdateBody { get; set; }
    }
}
