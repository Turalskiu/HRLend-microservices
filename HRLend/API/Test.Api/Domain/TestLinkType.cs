namespace TestApi.Domain
{
    public class TestLinkType
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public enum TEST_LINK_TYPE
    {
        FOR_USER = 1,
        FOR_GROUP = 2,
        FOR_ANONYMOUS = 3,
        FOR_ANONYMOUS_GROUP = 4
    }
}
