using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestApi.Attributes;
using TestApi.Domain;
using TestApi.Domain.DTO.Memory.TestPassage;
using TestApi.Domain.DTO.Response.TestPassage;
using TestApi.Domain.TestTemplateDocument;
using TTS = TestApi.Domain.TestTemplateStatisticsDocument;
using Helpers.Session;
using TestApi.Repository.GRPC;
using TestApi.Repository.SqlDB;
using TestApi.Services;
using DocumentDB = TestApi.Repository.DocumentDB;
using Contracts.Test.Queue;
using TestApi.Services.Queue.Publisher;


namespace TestApi.Controllers
{
    [Route("test/passage")]
    [ApiController]
    [Authorize]
    public class PassageController : ControllerBase
    {
        private ILinkRepository _linkRepository;
        private ITestRepository _testRepository;
        private DocumentDB.ITestRepository _testDocumentRepository;
        private ITestGeneratorRepository _testGeneratorRepository;
        private ITemplateStatisticsService _templateStatisticsService;
        private IStatisticPublisherService _statisticPublisherService;

        public PassageController(
            ILinkRepository linkRepository,
            DocumentDB.ITestRepository testDocumentRepository,
            ITestRepository testRepository,
            ITestGeneratorRepository testGeneratorRepository,
            ITemplateStatisticsService templateStatisticsService,
            IStatisticPublisherService statisticPublisherService
            )
        {
            _linkRepository = linkRepository;
            _testDocumentRepository = testDocumentRepository;
            _testRepository = testRepository;
            _testGeneratorRepository = testGeneratorRepository;
            _templateStatisticsService = templateStatisticsService;
            _statisticPublisherService = statisticPublisherService;
        }


        /// <summary>
        /// Страница начальной информации перед началом теста
        /// </summary>
        /// <remarks>
        /// 1 стадия: клиент переходит по ссылке и наблюдает информацию о тесте.
        /// Метод возвращает название теста, описание, лимит количества прохождения теста
        /// и текущий номер прохождения теста. Если текущий номер прохождения теста больше
        /// лимита, дальнейшие шаги можно не выполнять.
        /// </remarks>
        [HttpPost("info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(TestInfoResponse))]
        [SwaggerResponse(404, "Ссылка или тест не найден")]
        [SwaggerResponse(410, "Ссылка просрочен/закрыт/достигнут лимит")]
        public ActionResult Info(string link)
        {
            var userId = ((Domain.Auth.User)ControllerContext.HttpContext.Items["User"]).Id;

            TestLink? testLink = _linkRepository.GetTestLink(link);
            if (testLink == null) return NotFound("Ссылка не найдена");
            Test? test = _testRepository.GetTest(testLink.TestId);
            if (test == null) return NotFound("Тест не найден");

            int numberAttempt = _linkRepository.GetTestLinkResponseCount(testLink.Id, userId);

            if (testLink.Status.Id != (int)TEST_LINK_STATUS.OPEN)
            {
                ControllerContext.HttpContext.Session.Remove("test_session");
                if (testLink.Status.Id == (int)TEST_LINK_STATUS.LIMIT && numberAttempt == 0)
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

            TestSession session = new TestSession
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
                    LimitAttempt = testLink.LimitAttempt
                },
                Candidate = new CandidateInfo
                {
                    UserId = userId,
                    GroupId = testLink.Group?.Id
                }
            };
            numberAttempt++;

            session.NumberAttempt = numberAttempt;

            ControllerContext.HttpContext.Session.Set<TestSession>("test_session", session);

            return Ok(new TestInfoResponse
            {
                TestTitle = test.Title,
                TestDescription = test.Description,
                LimitAttempt = session.Link.LimitAttempt,
                NumberAttempt = numberAttempt
            });
        }


        /// <summary>
        /// Начать тест (для авторизованных пользователей)
        /// </summary>
        /// <remarks>
        /// 2 стадия: данный метод нужно вызывать если ссылка
        /// имеет тип FOR_USER или FOR_GROUP. ТИП ВОЗРАЩАЕМОГО
        /// ЗНАЧЕНИЯ СМОТРЕТЬ В FAG TestStartResponse.json ФАЙЛЕ.
        /// </remarks>
        [HttpPost("start/{link}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Ссылка не принадлежит данному пользователю/Количество попыток исчерпан")]
        [SwaggerResponse(404, "Ссылка не подтверждена")]
        [SwaggerResponse(429, "Слишком много запросов")]
        [SwaggerResponse(500, "Не удалось сгенерировать тест")]
        public async Task<ActionResult> Start(string link)
        {
            var userId = ((Domain.Auth.User)ControllerContext.HttpContext.Items["User"]).Id;

            TestSession? session = ControllerContext.HttpContext.Session.Get<TestSession>("test_session");

            if (session.ResponseId != 0) return StatusCode(429, "Тест уже был сгенерирован");
            if (session != null
                && session.Link.Link.Equals(link)
                && session.Link.TypeId != (int)TEST_LINK_TYPE.FOR_ANONYMOUS
                && session.Link.TypeId != (int)TEST_LINK_TYPE.FOR_ANONYMOUS_GROUP
                )
            {
                if(session.NumberAttempt > session.Link.LimitAttempt) return BadRequest("Количество попыток исчерпан");

                if (session.Link.TypeId == (int)TEST_LINK_TYPE.FOR_USER && session.Candidate.UserId != userId)
                    return BadRequest("Ссылка не принадлежит данному пользователю");

                //if (session.Link.TypeId == (int)TEST_LINK_TYPE.FOR_GROUP)
                //{
                //}

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
                    UserId = userId,
                    NumberAttempt = session.NumberAttempt
                });


                session.ResponseId = responseId;
                ControllerContext.HttpContext.Session.Set<TestSession>("test_session", session);

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
            TestSession session = ControllerContext.HttpContext.Session.Get<TestSession>("test_session");
            if (session != null && session.Link.Link.Equals(link) && session.ResponseId == response_id)
            {
                //если пользователь первый раз проходил тест, увеличиваем число людей которые прошли данный тест
                if(session.NumberAttempt == 1 )_linkRepository.IncreaseCandidateCountTestLink(session.Link.Id);

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

                DateTime date = DateTime.Now;
                UserStatisticQM userStatisticQM = new UserStatisticQM
                {
                    MessageType = (int)USER_STATISTIC_TYPE.TEST,
                    User = new UserQM
                    {
                        UserId = (int)session.Candidate.UserId
                    },
                    Competencies = statistics.Competencies.Select(c=>new CompetencyQM
                    {
                        Title = c.Title,
                        Percent = c.Percent,
                        DateCreate = date
                    }).ToHashSet(),
                    Skills = new HashSet<SkillQM>()
                };
                foreach(var c in statistics.Competencies)
                {
                    userStatisticQM.Skills.UnionWith(c.Skills.Select(s => new SkillQM
                    {
                        Title = s.Title,
                        Percent = s.Percent,
                        DateCreate = date
                    }).ToHashSet());
                }
                _statisticPublisherService.StatisticMessage(userStatisticQM);

                _linkRepository.InsertTestResult(new TestResult
                {
                    TestLinkResponseId = response_id,
                    IsPassed = result.UserResult.IsPassed,
                    TestResultLink = testResultLink,
                    TestTemplateStatisticsLink = testTemplateStatisticsLink
                });

                ControllerContext.HttpContext.Session.Remove("test_session");
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
            TestSession session = ControllerContext.HttpContext.Session.Get<TestSession>("test_session");
            if (session != null && session.Link.Link.Equals(link) && session.ResponseId == response_id)
            {
                //если пользователь первый раз проходил тест, увеличиваем число людей которые прошли данный тест
                if (session.NumberAttempt == 1) _linkRepository.IncreaseCandidateCountTestLink(session.Link.Id);

                //если достигнут лимит для данной ссылки, меняем статус ссылки
                if (session.Link.LimitCandidateCount != null && session.Link.LimitCandidateCount == session.Link.CandidateCount + 1)
                {
                    _linkRepository.UpdateTestLink(session.Link.Id, (int)TEST_LINK_STATUS.LIMIT);
                }
                _linkRepository.UpdateTestLinkResponse(session.ResponseId, (int)TEST_LINK_RESPONSE_STATUS.OVERDUE_TEST);

                ControllerContext.HttpContext.Session.Remove("test_session");
                return Ok();
            }
            else
                return NotFound("Ссылка не подтверждена");
        }
    }
}
