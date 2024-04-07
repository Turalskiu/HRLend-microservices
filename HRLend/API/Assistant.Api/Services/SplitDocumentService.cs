using Elastic.Clients.Elasticsearch.MachineLearning;

namespace Assistant.Api.Services
{

    public interface ISplitDocumentService
    {
        List<string> SplitDocument(string doc, int size);
    }

    public class SimpleSplitDocumentService : ISplitDocumentService
    {
        public List<string> SplitDocument(string doc, int size)
        {
            List<string> chunks = new List<string>();
            for (int i = 0; i < doc.Length; i += size)
            {
                if (i + size > doc.Length)
                    size = doc.Length - i;
                chunks.Add(doc.Substring(i, size));
            }
            return chunks;
        }

    }
}
