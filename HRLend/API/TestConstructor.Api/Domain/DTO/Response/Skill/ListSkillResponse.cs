namespace HRApi.Domain.DTO.Response.Skill
{
    public class ListSkillResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<SkillResponse> Skills { get; set; }
    }
}
