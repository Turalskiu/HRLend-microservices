namespace TestApi.Domain
{
    public class TestLinkStatus
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }


    public enum TEST_LINK_STATUS
    {
        OPEN = 1,
        CLOSED = 2,
        EXPIRED = 3,
        LIMIT = 4
    }
}
