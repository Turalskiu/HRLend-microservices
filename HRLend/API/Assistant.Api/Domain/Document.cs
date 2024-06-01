namespace Assistant.Api.Domain
{
    public class Document
    {
        public int Id { get; set; }
        public int CabinetId { get; set; }
        public string Title { get; set; }
        public string ElasticsearchIndex {  get; set; }

        public DocumentType Type { get; set; }  
    }
}
