
namespace HRApi.Domain
{
    public class TestTemplate
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        public string Title { get; set; }
        public List<TestTemplateAndCompetence> Competencies { get; set; }
    }
}
