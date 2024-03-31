using Grpc.Net.Client;
using TestApi.Domain.GRPC.TemplateGRPC;



namespace TestApi.Repository.GRPC
{

    public interface ITestTemplateRepository
    {
        Task<TestTemplate?> GetTestTemplate(int template_id, int cabinet_id);
    }

    public class TestTemplateRepository : ITestTemplateRepository
    {
        private readonly string _connectionString;
        public TestTemplateRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<TestTemplate?> GetTestTemplate(int template_id, int cabinet_id)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new Template.TemplateClient(channel);

            Id id = new Id
            {
                TemplateId = template_id,
                CabinetId = cabinet_id
            };

            TestTemplate template = await client.CreateTestTemplateAsync(id);
            return template;
        }
    }
}
