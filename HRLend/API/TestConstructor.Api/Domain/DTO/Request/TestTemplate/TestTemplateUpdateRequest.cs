namespace HRApi.Domain.DTO.Request.TestTemplate
{
    public class TestTemplateUpdateRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int[]? CompetenceIds { get; set; }
        public int[]? CompetenceNeedIds { get; set; }
    }
}
