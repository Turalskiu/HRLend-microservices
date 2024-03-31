namespace HRApi.Domain.DTO.Request.Competence
{
    public class CompetenceAddRequest
    {
        public string Title { get; set; }
        public int[]? SkillIds { get; set; }
        public int[]? SkillNeedIds { get; set; }
    }
}
