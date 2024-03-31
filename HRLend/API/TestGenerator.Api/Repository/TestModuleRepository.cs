using MongoDB.Bson;
using MongoDB.Driver;
using TestGeneratorApi.Domain.GRPC.TestModuleGRPC;



namespace TestGeneratorApi.Repository
{
    public interface ITestModuleRepository
    {
        Task<Module?> GetTestModule(string link);
        Task<IEnumerable<Module>> SelectTestModule(List<string> links);
        Task<string> InsertTestModule(Module testModule);
        Task<bool> UpdateTestModule(Module testModule);
        Task<bool> DeleteTestModule(string link);
    }

    public class TestModuleRepository : ITestModuleRepository
    {
        private readonly string _connectionString;
        private readonly string _db;

        public TestModuleRepository(string connectionString, string db)
        {
            _connectionString = connectionString;
            _db = db;
        }

        public async Task<Module?> GetTestModule(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Module>("test_module");

            var filter = Builders<Module>.Filter.Eq("_id", ObjectId.Parse(link));
            Module? result = await collection.Find(filter).FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<Module>> SelectTestModule(List<string> links)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Module>("test_module");

            var filter = Builders<Module>.Filter.In("_id", links.Select(id => ObjectId.Parse(id)));
            List<Module> result = await collection.Find(filter).ToListAsync();

            return result;
        }


        public async Task<string> InsertTestModule(Module testModule)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Module>("test_module");

            try
            {
                testModule.Id = ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(testModule);
                return testModule.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> UpdateTestModule(Module testModule)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Module>("test_module");

            var filterBuilder = Builders<Module>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(testModule.Id)),
                filterBuilder.Eq("options.is_default", false) // Проверка свойства is_default
            );

            UpdateDefinition<Module> update = Builders<Module>.Update.Set(x => x, testModule); 

            var result = await collection.ReplaceOneAsync(filter, testModule);

            if (result.IsModifiedCountAvailable && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
               return false;
            }
        }


        public async Task<bool> DeleteTestModule(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Module>("test_module");

            var filterBuilder = Builders<Module>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(link)),
                filterBuilder.Eq("options.is_default", false) // Проверка свойства is_default
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
