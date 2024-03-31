using MongoDB.Bson;
using MongoDB.Driver;
using TestApi.Domain.TestResultDocument;
using TestApi.Domain.TestTemplateDocument;
using TTS = TestApi.Domain.TestTemplateStatisticsDocument;


namespace TestApi.Repository.DocumentDB
{
    public interface ITestRepository
    {
        Task<TestTemplate?> GetTestTemplate(string link);
        Task<List<string>> GetTestTemplateRowsTestModuleId(string link);
        Task<string> InsertTestTemplate(TestTemplate template);
        Task<bool> DeleteTestTemplate(string link);

        Task<TestResult?> GetTestResult(string link);
        Task<string> InsertTestResult(TestResult result);
        Task<bool> DeleteTestResult(string link);

        Task<TTS.TemplateStatistics?> GetTestTemplateStatistics(string link);
        Task<string> InsertTestTemplateStatistics(TTS.TemplateStatistics statistics);
        Task<bool> DeleteTestTemplateStatistics(string link);
    }


    public class TestRepository : ITestRepository
    {
        private readonly string _connectionString;
        private readonly string _db;

        public TestRepository(string connectionString, string db)
        {
            _connectionString = connectionString;
            _db = db;
        }


        public async Task<TestTemplate?> GetTestTemplate(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestTemplate>("test_template");

            var filter = Builders<TestTemplate>.Filter.Eq("_id", ObjectId.Parse(link));
            TestTemplate? result = await collection.Find(filter).FirstOrDefaultAsync();

            return result;
        }
        public async Task<List<string>> GetTestTemplateRowsTestModuleId(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestTemplate>("test_template");

            var filter = Builders<TestTemplate>.Filter.Eq("_id", ObjectId.Parse(link));
            var projection = Builders<TestTemplate>.Projection.Include("competencies.skills.id_test_module");

            var result = await collection.Find(filter).Project(projection).FirstOrDefaultAsync();

            List<string> testModuleIds = new List<string>();

            if (result != null)
            {
                var competencies = result["competencies"].AsBsonArray;

                foreach (var competency in competencies)
                {
                    var skills = competency["skills"].AsBsonArray;

                    foreach (var skill in skills)
                    {
                        string testModuleId = skill["id_test_module"].AsString;
                        testModuleIds.Add(testModuleId);
                    }
                }
            }

            return testModuleIds;
        }
        public async Task<string> InsertTestTemplate(TestTemplate template)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestTemplate>("test_template");

            try
            {
                template.Id = ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(template);
                return template.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> DeleteTestTemplate(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestTemplate>("test_template");

            var filterBuilder = Builders<TestTemplate>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(link))
            );

            var result = await collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<TestResult?> GetTestResult(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestResult>("test_result");

            var filter = Builders<TestResult>.Filter.Eq("_id", ObjectId.Parse(link));
            TestResult? result = await collection.Find(filter).FirstOrDefaultAsync();

            return result;
        }
        public async Task<string> InsertTestResult(TestResult result)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestResult>("test_result");

            try
            {
                result.Id = ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(result);
                return result.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> DeleteTestResult(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TestResult>("test_result");

            var filterBuilder = Builders<TestResult>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(link))
            );

            var result = await collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<TTS.TemplateStatistics?> GetTestTemplateStatistics(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TTS.TemplateStatistics>("test_template_statistics");

            var filter = Builders<TTS.TemplateStatistics>.Filter.Eq("_id", ObjectId.Parse(link));
            TTS.TemplateStatistics? statistics = await collection.Find(filter).FirstOrDefaultAsync();

            return statistics;
        }
        public async Task<string> InsertTestTemplateStatistics(TTS.TemplateStatistics statistics)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TTS.TemplateStatistics>("test_template_statistics");

            try
            {
                statistics.Id = ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(statistics);
                return statistics.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> DeleteTestTemplateStatistics(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<TTS.TemplateStatistics>("test_template_statistics");

            var filterBuilder = Builders<TTS.TemplateStatistics>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(link))
            );

            var result = await collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
