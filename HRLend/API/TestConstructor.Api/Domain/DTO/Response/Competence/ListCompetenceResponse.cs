using HRApi.Domain.DTO.Response.Skill;

namespace HRApi.Domain.DTO.Response.Competence
{
    public class ListCompetenceResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<CompetenceShortResponse> Competencies { get; set; }
    }
}
