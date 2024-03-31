namespace HRApi.Domain.DTO.Request.Competence
{
    public class CompetenceUpdateRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int[]? SkillIds { get; set; }
        public int[]? SkillNeedIds { get; set; }
    }
}
