using HRApi.Domain.DTO.Request.Skill;

namespace HRApi.Domain.DTO.Request.Competence
{
    public class CompetenceConstructorRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? CompetenceNeed { get; set; }
        public bool IsUpdateBody { get; set; }
        public List<SkillConstructorRequest> Skills { get; set;}
    }
}
