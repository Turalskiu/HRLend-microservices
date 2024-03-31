namespace TestApi.Domain
{
    public class TestLinkResponseStatus
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }


    public enum TEST_LINK_RESPONSE_STATUS
    {
        RESPOND = 1,
        START_TEST = 2,
        END_TEST = 3,
        OVERDUE_TEST = 4
    }
}
