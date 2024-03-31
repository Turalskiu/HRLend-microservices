namespace HRApi.Domain
{
    public class TestTemplateAndCompetence
    {
        public int TestTemplateId { get; set; }
        public int CompetenceId { get; set; }
        public int CompetenceNeedId { get; set; }

        public TestTemplate TestTemplate { get; set; }
        public Competence Competence { get; set; }
        public CompetenceNeed CompetenceNeed { get; set; }
    }
}
