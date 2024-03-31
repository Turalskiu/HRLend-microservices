namespace HRApi.Domain.DTO.Request.TestTemplate
{
    public class TestTemplateAddRequest
    {
        public string Title { get; set; }
        public int[]? CompetenceIds { get; set; }
        public int[]? CompetenceNeedIds { get; set; }
    }
}
