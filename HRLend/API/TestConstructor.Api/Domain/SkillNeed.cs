namespace HRApi.Domain
{
    public class SkillNeed
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public enum SKILL_NEED
    {
        REQUIRE_HARD = 1,
        REQUIRE_MIDDLE = 2,
        REQUIRE_SOFT = 3
    }
}
