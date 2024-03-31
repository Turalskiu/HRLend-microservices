using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using TestApi.Attributes;
using TestApi.Domain.Auth;
using TestApi.Domain.DTO;
using TestApi.Domain.DTO.Request.Test;
using TestApi.Domain.DTO.Response.Test;
using TestApi.Domain.GRPC.TemplateGRPC;
using DocumentDB = TestApi.Repository.DocumentDB;
using TestApi.Repository.GRPC;
using TestApi.Repository.SqlDB;

namespace TestApi.Controllers
{
    [Authorize(Role = "cabinet_hr")]
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private ITestRepository _testRepository;
        private DocumentDB.ITestRepository _testDocumentRepository;
        private ITestTemplateRepository _templateRepository;

        public TestController(
            ITestRepository testRepository,
            DocumentDB.ITestRepository testDocumentRepository,
            ITestTemplateRepository templateRepository
            )
        {
            _testRepository = testRepository;
            _templateRepository = templateRepository;
            _testDocumentRepository = testDocumentRepository;
        }


        /// <summary>
        /// Создать тест
        /// </summary>
        [HttpPost]
        [Route("create")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Шаблон не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Не удалось создать тест")]
        public async Task<ActionResult> CreateTest(TestAddRequest test)
        {
            var userId = ((User)ControllerContext.HttpContext.Items["User"]).Id;
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            TestTemplate? template = await _templateRepository.GetTestTemplate(test.TestTemplateId, cabinetId);

            if (template == null) return BadRequest("Шаблон не найден");

            string link = await _testDocumentRepository.InsertTestTemplate(new Domain.TestTemplateDocument.TestTemplate
            {
                Title = template.Title,
                Settings = new Domain.TestTemplateDocument.Settings
                {
                    MixQuestions = test.Setings.MixGuestions,
                    MixAnswers = test.Setings.MixGuestions,
                    IsCorrect = test.Setings.IsCorrect,
                    IsTimer = test.Setings.IsTimer
                },
                ResultSettings = new Domain.TestTemplateDocument.ResultSettings
                {
                    IsShowRecommendMaterials = test.Setings.IsShowRecommendMaterials,
                    IsShowResult = test.Setings.IsShowResult,
                    IsShowResultAndTrueQuestions = test.Setings.IsShowResultAndTrueQuestions
                },
                Competencies = new List<Domain.TestTemplateDocument.Competency>(template.Competencies.Select(c =>
                {
                    return new Domain.TestTemplateDocument.Competency
                    {
                        Title = c.Title,
                        RequiredCode = c.CompetenceNeed,
                        Skills = new List<Domain.TestTemplateDocument.Skill>(c.Skills.Select(s =>
                        {
                            return new Domain.TestTemplateDocument.Skill
                            {
                                Title = s.Title,
                                RequiredCode = s.SkillNeed,
                                IdTestModule = s.TestModuleLink
                            };
                        }))
                    };
                }))
            });

            if (link != null) return Ok(_testRepository.InsertTest(new Domain.Test
            {
                Title = test.Title,
                Description = test.Description,
                HrId = userId,
                TestTemplateLink = link
            }));

            return StatusCode(500, "Не удалось создать тест");
        }


        /// <summary>
        /// Обновить тест
        /// </summary>
        [HttpPut]
        [Route("update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> UpdateTest(TestUpdateRequest test)
        {
            _testRepository.UpdateTest(new Domain.Test
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
            });
            return Ok();
        }


        /// <summary>
        /// Удалить тест
        /// </summary>
        [HttpDelete]
        [Route("delete")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> DeleteTest(int id)
        {
            string? link = _testRepository.GetTestRowTestTemplateLink(id);

            if(link != null)
            {
                _testDocumentRepository.DeleteTestTemplate(link);
                _testRepository.DeleteTest(id);
            }
            return Ok();
        }


        /// <summary>
        /// Получить тест
        /// </summary>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Успешный запрос", typeof(TestResponse))]
        [SwaggerResponse(400, "Тест не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetTest(int id)
        {
            Domain.Test? test = _testRepository.GetTestAndTestLink(id);

            if (test != null) 
            { 
                return Ok(new TestResponse
                {
                    Id = id,
                    Title = test.Title,
                    Description = test.Description,
                    TestTemplateLink = test.TestTemplateLink,
                    Links = test.Links.Select(l=> new Domain.DTO.Response.TestLink.TestLinkShortResponse
                    {
                        Id= l.Id,
                        Title = l.Title,
                        Status = l.Status
                    }).ToList()
                });
            }

            return BadRequest("Тест не найден");
        }


        /// <summary>
        /// Получить список тестов
        /// </summary>
        [HttpGet]
        [Route("page")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListTestResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetPageTests(int page_numb, int page_size, string sort)
        {
            var userId = ((User)ControllerContext.HttpContext.Items["User"]).Id;
            var tests = _testRepository.SelectTest(new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            }, 
            userId);


            return Ok(new ListTestResponse
            {
                TotalRows = tests.TotalRows,
                PageNumber = tests.PageNo,
                PageSize = tests.PageSize,
                Sort = tests.Sort,
                Tests = tests.Select(t => new TestShortResponse
                {
                    Id = t.Id,
                    Title = t.Title
                })
            });
        }


        /// <summary>
        /// Получить шаблон теста
        /// </summary>
        [HttpGet]
        [Route("template")]
        [SwaggerResponse(200, "Успешный запрос", typeof(Domain.TestTemplateDocument.TestTemplate))]
        [SwaggerResponse(400, "Шаблон не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetTemplateTest(string link)
        {
            Domain.TestTemplateDocument.TestTemplate? template = await _testDocumentRepository.GetTestTemplate(link);

            if (template != null) return Ok(template);

            return BadRequest("Шаблон не найден");
        }




    }
}
