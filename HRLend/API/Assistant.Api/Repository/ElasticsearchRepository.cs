using Assistant.Api.Domain.Elasticsearch;
using Nest;


namespace Assistant.Api.Repository
{

    public interface IElasticsearchRepository
    {
        Task InsertManyBlock(List<Block> blocks);
        Task DeleteBlock(string documentId);
        Task<List<Block>> FindBlocks(string documentId, string text, int count);
    }



    public class ElasticsearchRepository : IElasticsearchRepository
    {
        readonly string _url;
        readonly string _username;
        readonly string _password;
        readonly string _apiKeyId;
        readonly string _apiKey;
        readonly ElasticClient _client;
        readonly string _index;

        public ElasticsearchRepository(
            string url, 
            string username,
            string password,
            string index) 
        {
            _url = url;
            _username = username;
            _password = password;
            _index = index;

            var settings = new ConnectionSettings(new Uri(_url))
                .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
                .BasicAuthentication(_username, _password);
                //.ApiKeyAuthentication(_apiKeyId, _apiKey);

            _client = new ElasticClient(settings);
        }



        public async Task InsertManyBlock(List<Block> blocks)
        {
            var bulkResponse = await _client.BulkAsync(b => b
                .Index(_index)
                .IndexMany(blocks)
            );

            //if (bulkResponse.IsValid)
            //{
            //    Console.WriteLine("Документы успешно добавлены.");
            //}
            //else
            //{
            //    Console.WriteLine($"Ошибка при добавлении документов: {bulkResponse.DebugInformation}");
            //}
        }
        public async Task DeleteBlock(string documentId)
        {
            // Создание запроса на удаление документов по условию
            var deleteResponse = await _client.DeleteByQueryAsync<Block>(d => d
                .Index(_index)
                .Query(q => q
                    .Match(m => m
                        .Field("documentId")
                        .Query(documentId)
                    )
                )
            );

            //if (deleteResponse.IsValid) { }
        }
        public async Task<List<Block>> FindBlocks(string documentId, string text, int count)
        {
            var searchResponse = await _client.SearchAsync<Block>(s => s
                .Size(count)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(mt => mt
                                .Field(f => f.Content)
                                .Query(text)
                                .Fuzziness(Nest.Fuzziness.Auto)
                            ),
                            m => m
                                .MatchPhrase(mp => mp
                                    .Field(f => f.DocumentId)
                                    .Query(documentId)
                                )
                        )
                    )
                )
            );

            List<Block> blocks = new List<Block>();

            if (searchResponse.IsValid)
            {
                foreach (var hit in searchResponse.Hits)
                {
                    blocks.Add(new Block() 
                    {
                        DocumentId = documentId,
                        Content = hit.Source.Content
                    });
                    Console.WriteLine($"Score: {hit.Score}, DocumentId: {hit.Source.DocumentId}");
                }
            }
            //else
            //{
            //    Console.WriteLine($"Ошибка при выполнении запроса: {searchResponse.DebugInformation}");
            //}

            return blocks;
        }


        public static async Task<string> CreateIndex(
            string url,
            string username,
            string password,
            string index
            )
        {
            var settings = new ConnectionSettings(new Uri(url))
                .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
                .BasicAuthentication(username, password);

            ElasticClient client = new ElasticClient(settings);

            var indexExistsResponse = await client.Indices.ExistsAsync(index);

            if (!indexExistsResponse.Exists)
            {
                var createIndexResponse = await client.Indices.CreateAsync(index, c => c
                    .Map<Block>(m => m
                        .AutoMap()
                    )
                );

                if (createIndexResponse.IsValid)
                    return index;
            }

            return null;
        }
        public static async Task DeleteIndex(
            string url,
            string username,
            string password,
            string index
            )
        {
            var settings = new ConnectionSettings(new Uri(url))
                .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
                .BasicAuthentication(username, password);

            ElasticClient client = new ElasticClient(settings);

            var deleteIndexResponse = await client.Indices.DeleteAsync(index);

            //if (deleteIndexResponse.IsValid)
            //{
            //    Console.WriteLine($"Индекс {index} успешно удален.");
            //}
            //else
            //{
            //    Console.WriteLine($"Ошибка при удалении индекса {index}: {deleteIndexResponse.DebugInformation}");
            //}
        }
    }
}
