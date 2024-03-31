using HRApi.Domain.DTO.Response.Skill.ForCompetence;

namespace HRApi.Domain.DTO.Response.Competence
{
    public class CompetenceResponse
    {
        public int Id { get; set; }
        public string Title { get; set;}
        public IEnumerable<SkillShortForCompetenceResponse> Skills { get; set; }
    }
}
