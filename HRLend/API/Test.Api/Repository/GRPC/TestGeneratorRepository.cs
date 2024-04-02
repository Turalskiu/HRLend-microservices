using Grpc.Net.Client;
using Contracts.TestGenerator.GRPC.TestGenerator;


namespace TestApi.Repository.GRPC
{
    public interface ITestGeneratorRepository
    {
        Task<Test> GetTest(string[] links,
            bool mixQuestions,
            bool mixAnswers,
            bool isCorrect,
            bool isTimer,
            bool isRecommendMaterials
            );
    }

    public class TestGeneratorRepository : ITestGeneratorRepository
    {
        private readonly string _connectionString;
        public TestGeneratorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Test> GetTest(string[] links,
            bool mixQuestions,
            bool mixAnswers,
            bool isCorrect,
            bool isTimer,
            bool isRecommendMaterials
            )
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new TestGenerator.TestGeneratorClient(channel);

            ParamForTestGenerator param = new ParamForTestGenerator
            {
                IsCorrect = isCorrect,
                IsTimer = isTimer,
                MixAnswers = mixAnswers,
                MixQuestions = mixQuestions,
                IsRecommendMaterials = isRecommendMaterials
            };
            param.TestModuleLinks.AddRange(links);

            Test test = await client.GenerateTestAsync(param);
            return test;
        }
    }
}
