using HRApi.Domain.DTO.Response.Competence.ForTestTemplate;

namespace HRApi.Domain.DTO.Response.TestTemplate
{
    public class TestTemplateResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<CompetenceShortForTestTemplateResponse> Competencies { get; set; }
    }
}
