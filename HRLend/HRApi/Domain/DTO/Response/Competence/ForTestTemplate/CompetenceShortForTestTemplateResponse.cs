using HRApi.Domain.DTO.Response.Skill.ForCompetence;

namespace HRApi.Domain.DTO.Response.Competence.ForTestTemplate
{
    public class CompetenceShortForTestTemplateResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int NeedId { get; set; }
        public string NeedTitle { get; set; }
        public IEnumerable<SkillShortForCompetenceResponse> Skills { get; set; }
    }
}
