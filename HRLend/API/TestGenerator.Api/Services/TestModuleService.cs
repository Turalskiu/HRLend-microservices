﻿using Grpc.Core;
using Contracts.TestGenerator.GRPC.TestModule;
using TestGeneratorApi.Repository;



namespace TestGeneratorApi.Services
{
    public class TestModuleService : TestModule.TestModuleBase
    {
        private ITestModuleRepository _testModuleRepository;

        public TestModuleService(
            ITestModuleRepository testModuleRepository
            )
        {
            _testModuleRepository = testModuleRepository;
        }

        public override async Task<Module?> GetTestModule(Link link, ServerCallContext context)
        {
            Module? module = await _testModuleRepository.GetTestModule(link.Link_);
            if (module != null) 
                return await Task.FromResult(module);
            throw new RpcException(new Status(StatusCode.NotFound, "Модуль не найден"));
        }

        public override async Task<Link> InsertTestModule(Module testModule, ServerCallContext context)
        {
            return await Task.FromResult(new Link
                {
                    Link_ = await _testModuleRepository.InsertTestModule(testModule)
                }  
            );
        }

        public override async Task<ResultModification> UpdateTestModule(Module testModule, ServerCallContext context)
        {
            ResultModification res = new ResultModification
            {
                IsCreate = false
            };

            bool IsDefault = await _testModuleRepository.IsDefaultTestModule(testModule.Id);

            if (!IsDefault)
            {
                await _testModuleRepository.UpdateTestModule(testModule);
                return res;
            }

            string link = await _testModuleRepository.InsertTestModule(testModule);
            res.IsCreate = true;
            res.NewTestModuleLink = link;

            return res;
        }

        public override async Task<ResultDelete> DeleteTestModule(Link link, ServerCallContext context)
        {
            return await Task.FromResult(new ResultDelete
                {
                    Result = await _testModuleRepository.DeleteTestModule(link.Link_)
                }
            );
        }
    }
}
