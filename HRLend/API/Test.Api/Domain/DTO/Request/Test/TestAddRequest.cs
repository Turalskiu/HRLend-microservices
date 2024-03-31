
namespace TestApi.Domain.DTO.Request.Test
{
    public class TestAddRequest 
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TestSetingsRequest Setings { get; set; }
        public int TestTemplateId { get; set; }
    }
}
