namespace Assistant.Api.Domain.DTO.Request
{
    public class DocumentAddRequest
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Type { get; set; }
    }
}
