using Grpc.Core;
using TG = TestGeneratorApi.Domain.GRPC.TestGeneratorGRPC;
using TM = TestGeneratorApi.Domain.GRPC.TestModuleGRPC;
using TestGeneratorApi.Repository;
using Helpers.Collections;

namespace TestGeneratorApi.Services
{
    public class TestGeneratorService : TG.TestGenerator.TestGeneratorBase
    {
        private ITestModuleRepository _testModuleRepository;

        public TestGeneratorService(
            ITestModuleRepository testModuleRepository
            )
        {
            _testModuleRepository = testModuleRepository;
        }

        public override async Task<TG.Test?> GenerateTest(TG.ParamForTestGenerator param, ServerCallContext context)
        {
            IEnumerable<TM.Module> modules = await _testModuleRepository.SelectTestModule(param.TestModuleLinks.ToList());
            return await Task.FromResult(CreateTest(param, modules));
        }

 
        private TG.Test CreateTest(TG.ParamForTestGenerator param, IEnumerable<TM.Module> modules)
        {
            int count_questions = 0;
            int max_value = 0;
            int limit_duration_in_seconds = 0;

            TG.Test test = new TG.Test 
            { 
                Settings = new TG.Settings
                {
                    IsCorrected = param.IsCorrect,
                    IsTimer = param.IsTimer
                },
                TestInfo = new TG.TestInfo(),
                Rule = new TG.Rule()
            };


            foreach (var module in modules)
            {
                string id_test_module = module.Id;

                limit_duration_in_seconds += module.Options.LimitDurationInSeconds;

                TG.TestModule tm = new TG.TestModule
                {
                    Id = id_test_module,
                    MinValueForPassed = module.Rule.MinValueForPassed,
                };
                if (param.IsRecommendMaterials)
                {
                    tm.Recommendations.AddRange(module.Recommendations.Select(r=> new TG.Recommendation
                    {
                        Title = r.Title,
                        Description = r.Description,
                        Link = r.Link
                    }));
                }
                test.Rule.TestModules.Add(tm);

                //добавляем вопросы
                int take_question = module.Options.TakeQuestions;
                count_questions += take_question;
                List<TM.Question> list_question = module.Questions.Shuffle().ToList();
                for(int i = 0; i < take_question; i++)
                {
                    TM.Question q = list_question[i];

                    TG.Question qq = new TG.Question
                    {
                        TestModuleId = id_test_module,
                        Text = q.Text,
                        Description = q.Description,
                        Type = q.Type,
                        MaxValue= q.MaxValue
                    };

                    max_value += q.MaxValue;

                    foreach (var o in q.Options)
                    {
                        TG.Option oo = new TG.Option
                        {
                            IsTrue = o.IsTrue,
                            Text = o.Text
                        };
                        qq.Options.Add(oo);
                    }

                    if (param.MixAnswers) ;
                        qq.OptionsList = new List<TG.Option>(qq.Options.Shuffle());
                    
                    test.Questions.Add(qq);
                }
            }
            if (param.MixQuestions)
                test.QuestionsList = new List<TG.Question>(test.Questions.Shuffle());

            
            test.Settings.LimitDurationInSeconds = limit_duration_in_seconds;
            test.TestInfo.MaxValue = max_value;
            test.TestInfo.CountQuestions = count_questions;

            return test;
        }
    }
}
