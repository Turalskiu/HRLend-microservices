using HRApi.Domain.DTO.Request.Competence;
using HRApi.Domain.DTO.Request.Skill;

namespace HRApi.Domain.DTO.Request.TestTemplate
{
    public class TemplateConstructorRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsUpdateBody { get; set; }
        public List<CompetenceConstructorRequest> Competencies { get; set; }
    }
}
