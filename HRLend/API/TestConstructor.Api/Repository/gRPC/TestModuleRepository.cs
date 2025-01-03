﻿using Grpc.Net.Client;
using Contracts.TestGenerator.GRPC.TestModule;

namespace HRApi.Repository.gRPC
{
    public interface ITestModuleRepository
    {
        Task<Module?> GetTestModule(string id);
        Task<string> InsertTestModule(Module module);
        Task<ResultModification> UpdateTestModule(Module module);
        Task<ResultDelete> DeleteTestModule(string id);
    }

    public class TestModuleRepository : ITestModuleRepository
    {
        private readonly string _connectionString;
        public TestModuleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Module?> GetTestModule(string id)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new TestModule.TestModuleClient(channel);

            Link link = new Link { Link_ = id };

            Module testModule = await client.GetTestModuleAsync(link);
            return testModule;
        }

        public async Task<string> InsertTestModule(Module module)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new TestModule.TestModuleClient(channel);

            Link result = await client.InsertTestModuleAsync(module);
            return result.Link_;
        }

        public async Task<ResultModification> UpdateTestModule(Module module)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new TestModule.TestModuleClient(channel);

            ResultModification result = await client.UpdateTestModuleAsync(module);
            return result;
        }

        public async Task<ResultDelete> DeleteTestModule(string id)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new TestModule.TestModuleClient(channel);

            ResultDelete result = await client.DeleteTestModuleAsync(new Link { Link_ = id });
            return result;
        }
    }
}
