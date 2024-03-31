namespace HRApi.Domain
{
    public class CompetenceNeed
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public enum COMPETENCE_NEED
    {
        REQUIRE_HARD = 1,
        REQUIRE_MIDDLE = 2,
        REQUIRE_SOFT = 3
    }
}
