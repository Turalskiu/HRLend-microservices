namespace Assistant.Api.Domain
{
    public class DocumentType
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public enum DOCUMENT_TYPE
    {
        FOR_TEST = 1,
        FOR_COMPANY_INFO = 2
    }
}
