using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestApi.Domain;
using TestApi.Domain.DTO.Memory.TestPassage;
using TestApi.Domain.DTO.Request.TestPassage;
using TestApi.Domain.DTO.Response.TestPassage;
using TestApi.Domain.TestTemplateDocument;
using TTS = TestApi.Domain.TestTemplateStatisticsDocument;
using TestApi.Repository.GRPC;
using TestApi.Repository.SqlDB;
using TestApi.Services;
using Helpers.Session;
using DocumentDB = TestApi.Repository.DocumentDB;
using TestApi.Domain.DTO.Memory.TestPassage.Anon;
using TestApi.Domain.DTO.Response.TestPassage.Anon;

namespace TestApi.Controllers
{
    [Route("test/anon-passage")]
    [ApiController]
    public class AnonPassageController : ControllerBase
    {
        private ILinkRepository _linkRepository;
        private ITestRepository _testRepository;
        private DocumentDB.ITestRepository _testDocumentRepository;
        private ITestGeneratorRepository _testGeneratorRepository;
        private ITemplateStatisticsService _templateStatisticsService;
        private IMailService _mailService;

        public AnonPassageController(
            ILinkRepository linkRepository,
            DocumentDB.ITestRepository testDocumentRepository,
            ITestRepository testRepository,
            ITestGeneratorRepository testGeneratorRepository,
            ITemplateStatisticsService templateStatisticsService,
            IMailService mailService
            )
        {
            _linkRepository = linkRepository;
            _testDocumentRepository = testDocumentRepository;
            _testRepository = testRepository;
            _testGeneratorRepository = testGeneratorRepository;
            _templateStatisticsService = templateStatisticsService;
            _mailService = mailService;
        }



        /// <summary>
        /// Узнать тип ссылки
        /// </summary>
        /// <remarks>
        /// 0 стадия: перед началом работы с тестом требуется
        /// узнать тип ссылки (для анонимного пользователя или авторизованного)
        /// Для данного метода авторизация не требуется.
        /// </remarks>
        [HttpGet("type-link")]
        [SwaggerResponse(200, "Успешный запрос", typeof(int))]
        [SwaggerResponse(404, "Ссылка не найден")]
        [SwaggerResponse(410, "Ссылка просрочен/закрыт/достигнут лимит")]
        public ActionResult GetTypeLink(string link)
        {

            TestLink? testLink = _linkRepository.GetTestLink(link);
            if (testLink == null) return NotFound("Ссылка не найдена");

            if (testLink.Status.Id != (int)TEST_LINK_STATUS.OPEN)
            {
                ControllerContext.HttpContext.Session.Remove("test_session");
                if (testLink.Status.Id == (int)TEST_LINK_STATUS.LIMIT)
                {
                    return StatusCode(410, "Исчерпан лимит кандидатов");
                }
                else if (testLink.Status.Id == (int)TEST_LINK_STATUS.CLOSED)
                {
                    return StatusCode(410, "Ссылка закрыта");
                }
                else if (testLink.Status.Id == (int)TEST_LINK_STATUS.EXPIRED)
                {
                    return StatusCode(410, "Ссылка просрочена");
                }
            }

            if (testLink.DateExpired <= DateTime.UtcNow)
            {
                _linkRepository.UpdateTestLink(testLink.Id, (int)TEST_LINK_STATUS.EXPIRED);
                return StatusCode(410, "Ссылка просрочена");
            }

            return Ok(testLink.Type.Id);
        }




        /// <summary>
        /// Страница начальной информации перед началом теста
        /// </summary>
        /// <remarks>
        /// 1 стадия: клиент переходит по ссылке и наблюдает информацию о тесте.
        /// Метод возвращает название теста, описание
        /// </remarks>
        [HttpPost("info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(AnonTestInfoResponse))]
        [SwaggerResponse(404, "Ссылка или тест не найден")]
        [SwaggerResponse(410, "Ссылка просрочен/закрыт/достигнут лимит")]
        public ActionResult Info(string link)
        {

            TestLink? testLink = _linkRepository.GetTestLink(link);
            if (testLink == null) return NotFound("Ссылка не найдена");
            Test? test = _testRepository.GetTest(testLink.TestId);
            if (test == null) return NotFound("Тест не найден");

            if (testLink.Status.Id != (int)TEST_LINK_STATUS.OPEN)
            {
                ControllerContext.HttpContext.Session.Remove("test_session");
                if (testLink.Status.Id == (int)TEST_LINK_STATUS.LIMIT)
                {
                    return StatusCode(410, "Исчерпан лимит кандидатов");
                }
                else if (testLink.Status.Id == (int)TEST_LINK_STATUS.CLOSED)
                {
                    return StatusCode(410, "Ссылка закрыта");
                }
                else if (testLink.Status.Id == (int)TEST_LINK_STATUS.EXPIRED)
                {
                    return StatusCode(410, "Ссылка просрочена");
                }
            }

            if (testLink.DateExpired <= DateTime.UtcNow)
            {
                _linkRepository.UpdateTestLink(testLink.Id, (int)TEST_LINK_STATUS.EXPIRED);
                return StatusCode(410, "Ссылка просрочена");
            }

            AnonTestSession session = new AnonTestSession
            {
                DateCreate = DateTime.UtcNow,
                Test = new TestInfo
                {
                    Id = test.Id,
                    Title = test.Title,
                    Description = test.Description,
                    TestTemplateLink = test.TestTemplateLink
                },
                Link = new LinkInfo
                {
                    Id = testLink.Id,
                    TypeId = testLink.Type.Id,
                    StatusId = testLink.Status.Id,
                    DateExpired = testLink.DateExpired,
                    Link = testLink.Link,
                    CandidateCount = testLink.CandidateCount,
                    LimitCandidateCount = testLink.LimitCandidateCount,
                }
            };
            ControllerContext.HttpContext.Session.Set<AnonTestSession>("anon_test_session", session);

            return Ok(new AnonTestInfoResponse
            {
                TestTitle = test.Title,
                TestDescription = test.Description
            });
        }


        /// <summary>
        /// Заполнение данных о пользователи
        /// </summary>
        /// <remarks>
        /// 2 стадия: заполняются данные об пользователи,
        /// на почту отправляется код для подтверждения
        /// </remarks>
        [HttpPost("send-code")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Пользователь с такой почтой уже проходил тест")]
        [SwaggerResponse(404, "Ссылка не найдена")]
        [SwaggerResponse(410, "Ссылка просрочен/закрыт/достигнут лимит")]
        public ActionResult SendCode(string link, AnonymousCandidateRequest candidate)
        {
            AnonTestSession? session = ControllerContext.HttpContext.Session.Get<AnonTestSession>("anon_test_session");

            if(session != null && session.Link.Link.Equals(link)) 
            {
                if (_linkRepository.IsCheckPassingTestLinkByEmail(session.Link.Id, candidate.Email))
                    return BadRequest("Пользователь с такой почтой уже проходил тест");

                AnonCandidateInfo can = new AnonCandidateInfo
                {
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    MiddleName = candidate.MiddleName,
                    Email = candidate.Email
                };

                string code = _mailService.SendCodeForPassageTest(candidate.Email);
                can.Code = code;

                ControllerContext.HttpContext.Session.Set<AnonCandidateInfo>("anon_candidate_session", can);

                return Ok();
            }

            return NotFound("Ссылка не подтверждена");
        }



        /// <summary>
        /// Начать тест (для анонимных пользователей)
        /// </summary>
        /// <remarks>
        /// 3 стадия: данный метод нужно вызывать если ссылка
        /// имеет тип FOR_ANONYMOUS_USER или FOR_ANONYMOUS_GROUP.
        /// ТИП ВОЗРАЩАЕМОГО ЗНАЧЕНИЯ СМОТРЕТЬ В FAG TestStartResponse.json ФАЙЛЕ.
        /// </remarks>
        [HttpPost("start/{link}/{code}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не верный код")]
        [SwaggerResponse(404, "Ссылка не подтверждена")]
        [SwaggerResponse(429, "Слишком много запросов")]
        [SwaggerResponse(500, "Не удалось сгенерировать тест")]
        public async Task<ActionResult> AnonStart(string link, string code)
        {
            AnonTestSession? session = ControllerContext.HttpContext.Session.Get<AnonTestSession>("anon_test_session");
            AnonCandidateInfo? candidate = ControllerContext.HttpContext.Session.Get<AnonCandidateInfo>("anon_candidate_session");

            if (candidate == null || !code.Equals(candidate.Code)) return BadRequest("Код не верный");

            if (session.ResponseId != 0) return StatusCode(429, "Тест уже был сгенерирован");
            if (session != null
                && session.Link.Link.Equals(link)
                && session.Link.TypeId != (int)TEST_LINK_TYPE.FOR_USER
                && session.Link.TypeId != (int)TEST_LINK_TYPE.FOR_GROUP
                )
            {
                //получаем шаблон теста
                TestTemplate? template = await _testDocumentRepository.GetTestTemplate(session.Test.TestTemplateLink);

                HashSet<string> testModuleLinks = new HashSet<string>();
                foreach (var c in template.Competencies)
                {
                    foreach (var s in c.Skills) testModuleLinks.Add(s.IdTestModule);
                }

                //создаем тест
                Contracts.TestGenerator.GRPC.TestGenerator.Test test
                    = await _testGeneratorRepository.GetTest(
                        testModuleLinks.ToArray(),
                        template.Settings.MixQuestions,
                        template.Settings.MixAnswers,
                        template.Settings.IsCorrect,
                        template.Settings.IsTimer,
                        template.ResultSettings.IsShowRecommendMaterials
                        );

                int responseId = _linkRepository.InsertTestLinkResponse(new TestLinkResponse
                {
                    TestLinkId = session.Link.Id,
                    StatusId = (int)TEST_LINK_RESPONSE_STATUS.START_TEST,
                    UserId = null,
                    NumberAttempt = 1
                });

                int anonUserId = _linkRepository.InsertAnonymousUser(new AnonymousUser
                {
                    TestLinkResponseId = responseId,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    MiddleName = candidate.MiddleName,
                    Email = candidate.Email
                });

                session.ResponseId = responseId;
                session.AnonUserId = anonUserId;
                ControllerContext.HttpContext.Session.Set<AnonTestSession>("anon_test_session", session);

                ControllerContext.HttpContext.Session.Remove("anon_candidate_session");

                return Ok(new TestStartResponse
                {
                    ResponseId = responseId,
                    ResultSetings = new TestResultSetingsResponse
                    {
                        IsShowResultAndTrueQuestions = template.ResultSettings.IsShowResultAndTrueQuestions,
                        IsShowResult = template.ResultSettings.IsShowResult,
                        IsShowRecommendMaterials = template.ResultSettings.IsShowRecommendMaterials
                    },
                    Test = test
                });

            }
            return NotFound("Ссылка не подтверждена");
        }


        /// <summary>
        /// Завершить тест
        /// </summary>
        [HttpPost]
        [Route("end/{link}/{response_id}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Ссылка не подтверждена")]
        public async Task<ActionResult> End(string link, int response_id, TestApi.Domain.TestResultDocument.TestResult result)
        {
            AnonTestSession session = ControllerContext.HttpContext.Session.Get<AnonTestSession>("anon_test_session");
            if (session != null && session.Link.Link.Equals(link) && session.ResponseId == response_id)
            {
                _linkRepository.IncreaseCandidateCountTestLink(session.Link.Id);
                //если достигнут лимит для данной ссылки, меняем статус ссылки
                if (session.Link.LimitCandidateCount != null && session.Link.LimitCandidateCount == session.Link.CandidateCount + 1)
                {
                    _linkRepository.UpdateTestLink(session.Link.Id, (int)TEST_LINK_STATUS.LIMIT);
                }
                _linkRepository.UpdateTestLinkResponse(session.ResponseId, (int)TEST_LINK_RESPONSE_STATUS.END_TEST);

                string testResultLink = await _testDocumentRepository.InsertTestResult(result);

                TestTemplate? template = await _testDocumentRepository.GetTestTemplate(session.Test.TestTemplateLink);
                //получаем статистику относительно шаблона по пройденному тесту
                TTS.TemplateStatistics statistics = _templateStatisticsService.CreateTemplateStatistics(template, result);

                string testTemplateStatisticsLink = await _testDocumentRepository.InsertTestTemplateStatistics(statistics);

                _linkRepository.InsertTestResult(new TestResult
                {
                    TestLinkResponseId = response_id,
                    IsPassed = result.UserResult.IsPassed,
                    TestResultLink = testResultLink,
                    TestTemplateStatisticsLink = testTemplateStatisticsLink
                });

                ControllerContext.HttpContext.Session.Remove("anon_test_session");
                return Ok();
            }
            else
                return NotFound("Ссылка не подтверждена");
        }


        /// <summary>
        /// Тест просрочен
        /// </summary>
        [HttpPost]
        [Route("expire/{link}/{response_id}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Ссылка не подтверждена")]
        public ActionResult Expire(string link, int response_id)
        {
            AnonTestSession session = ControllerContext.HttpContext.Session.Get<AnonTestSession>("anon_test_session");
            if (session != null && session.Link.Link.Equals(link) && session.ResponseId == response_id)
            {
                _linkRepository.IncreaseCandidateCountTestLink(session.Link.Id);
                //если достигнут лимит для данной ссылки, меняем статус ссылки
                if (session.Link.LimitCandidateCount != null && session.Link.LimitCandidateCount == session.Link.CandidateCount + 1)
                {
                    _linkRepository.UpdateTestLink(session.Link.Id, (int)TEST_LINK_STATUS.LIMIT);
                }
                _linkRepository.UpdateTestLinkResponse(session.ResponseId, (int)TEST_LINK_RESPONSE_STATUS.OVERDUE_TEST);

                ControllerContext.HttpContext.Session.Remove("anon_test_session");
                return Ok();
            }
            else
                return NotFound("Ссылка не подтверждена");
        }
    }
}
