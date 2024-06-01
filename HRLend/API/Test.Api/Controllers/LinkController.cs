using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.WireProtocol.Messages.Encoders;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Text.RegularExpressions;
using TestApi.Attributes;
using TestApi.Domain;
using TestApi.Domain.Auth;
using TestApi.Domain.DTO.Request.Test;
using TestApi.Domain.DTO.Request.TestLink;
using TestApi.Domain.DTO.Response.AnonymousUser;
using TestApi.Domain.DTO.Response.Test;
using TestApi.Domain.DTO.Response.TestLink;
using TestApi.Domain.DTO.Response.TestLinkResponse;
using TestApi.Domain.DTO.Response.TestResult;
using TestApi.Repository.SqlDB;
using DocumentDB = TestApi.Repository.DocumentDB;



namespace TestApi.Controllers
{
    [Authorize(Role = "cabinet_hr")]
    [Route("test/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private ILinkRepository _linkRepository;
        private DocumentDB.ITestRepository _testDocumentRepository;

        public LinkController(
            ILinkRepository linkRepository,
            DocumentDB.ITestRepository testDocumentRepository
            )
        {
            _linkRepository = linkRepository;
            _testDocumentRepository = testDocumentRepository;
        }


        /// <summary>
        /// Создать ссылку на тест
        /// </summary>
        [HttpPost]
        [Route("create")]
        [SwaggerResponse(200, "Успешный запрос", typeof(string))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Ошибка, не удалось создать ссылку")]
        public async Task<ActionResult> CreateLink(TestLinkAddResponse link)
        {
            string l = Guid.NewGuid().ToString();

            int result = _linkRepository.InsertTestLink(new Domain.TestLink
                {
                    TestId = link.TestId,
                    Title = link.Title,
                    TypeId = link.TypeId,
                    UserId = link.CandidateId,
                    GroupId = link.GroupId,
                    LimitCandidateCount = link.LimitCandidateCount,
                    LimitAttempt = link.LimitAttempt,
                    DateExpired = link.DateExpired,
                    StatusId = (int)TEST_LINK_STATUS.OPEN,
                    Link = l
                });

            if (result > 0) return Ok(l);
            return StatusCode(500, "Не удалось создать ссылку");
        }


        /// <summary>
        /// Закрыть ссылку
        /// </summary>
        [HttpDelete]
        [Route("close")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> CloseLink(int id)
        {
            _linkRepository.ClosedTestLink(id);
            return Ok();
        }

        /// <summary>
        /// Удалить ссылку
        /// </summary>
        [HttpDelete]
        [Route("delete")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> DeleteLink(int id)
        {
            _linkRepository.DeleteTestLink(id);
            return Ok();
        }


        /// <summary>
        /// Получить ссылку
        /// </summary>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Успешный запрос", typeof(TestApi.Domain.DTO.Response.TestLink.TestLinkResponse))]
        [SwaggerResponse(400, "ссылка не найдена")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetLink(int id)
        {
            Domain.TestLink? link = _linkRepository.GetTestLink(id);

            if (link != null)
            {
                return Ok(new TestApi.Domain.DTO.Response.TestLink.TestLinkResponse
                {
                    Id = link.Id,
                    Candidate = link.Candidate,
                    Group = link.Group,
                    LimitCandidateCount = link.LimitCandidateCount,
                    LimitAttempt = link.LimitAttempt,
                    CandidateCount = link.CandidateCount,
                    Title = link.Title,
                    Link = link.Link,
                    DateCreate = link.DateCreate,
                    DateClosed = link.DateClosed,
                    DateExpired = link.DateExpired,
                    Status = link.Status,
                    Type = link.Type,
                });
            }

            return BadRequest("Ссылка не найдена");
        }


        /// <summary>
        /// Получить набор ссылок
        /// </summary>
        [HttpGet]
        [Route("page")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListTestLinkResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetPageLinks(int page_numb, int page_size, string sort)
        {
            var userId = ((Domain.Auth.User)ControllerContext.HttpContext.Items["User"]).Id;

            var links = _linkRepository.SelectTestLink(new Domain.DTO.Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            },
            userId);


            return Ok(new ListTestLinkResponse
            {
                TotalRows = links.TotalRows,
                PageNumber = links.PageNo,
                PageSize = links.PageSize,
                Sort = links.Sort,
                Links = links.Select(l => new TestLinkShortResponse
                {
                    Id = l.Id,
                    Title = l.Title,
                    Status = l.Status
                })
            });
        }



        /// <summary>
        /// Получить подробную информацию отклика на ссылку
        /// </summary>
        [HttpGet]
        [Route("response")]
        [SwaggerResponse(200, "Успешный запрос", typeof(TestLinkResponseResponse))]
        [SwaggerResponse(400, "Отклик не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetResponse(int id)
        {
            Domain.TestLinkResponse? response = _linkRepository.GetTestLinkResponseAndUser(id);

            if (response != null)
            {
                var result = new TestLinkResponseResponse
                {
                    Id = response.Id,
                    Candidate= response.Candidate,
                    DateCreate = response.DateCreate,
                    TestGeneratedLink = response.TestGeneratedLink,
                    Status = response.Status,
                    NumberAttempt = response.NumberAttempt
                };

                if(response.AnonymousCandidate !=  null)
                {
                    result.AnonymousCandidate = new AnonymousUserResponse
                    {
                        FirstName = response.AnonymousCandidate.FirstName,
                        LastName = response.AnonymousCandidate.LastName,
                        MiddleName = response.AnonymousCandidate.MiddleName,
                        Email = response.AnonymousCandidate.Email
                    };
                }

                if (response.TestResult != null)
                {
                    result.TestResult = new TestResultResponse
                    {
                        IsPassed = response.TestResult.IsPassed,
                        TestResultLink = response.TestResult.TestResultLink,
                        TestTemplateStatisticstLink = response.TestResult.TestTemplateStatisticsLink
                    };
                }

                return Ok(result);
            }

            return BadRequest("Отклик не найден");
        }


        /// <summary>
        /// Получить набор откликов
        /// </summary>
        [HttpGet]
        [Route("response/page")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListTestLinkResponseResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetPageResponses(int test_link_id, int page_numb, int page_size, string sort)
        {
            var responses = _linkRepository.SelectTestLinkResponseAndUser(new Domain.DTO.Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            },
            test_link_id);

            var result = new ListTestLinkResponseResponse
            {
                TotalRows = responses.TotalRows,
                PageNumber = responses.PageNo,
                PageSize = responses.PageSize,
                Sort = responses.Sort,
                Responses = responses.Select(r => {
                    var res = new TestLinkResponseShortResponse
                    {
                        Id = r.Id,
                        DateCreate = r.DateCreate,
                        Status = r.Status,
                        NumberAttempt = r.NumberAttempt
                    }; 

                    if(r.Candidate != null)
                    {
                        res.Candidate = new Domain.User
                        {
                            Id = r.Candidate.Id,
                            Username = r.Candidate.Username,
                            Email = r.Candidate.Email,
                            Photo = r.Candidate.Photo
                        };
                    }
                    else
                    {
                        res.AnonymousCandidate = new AnonymousUserShortResponse
                        {
                            FirstName = r.AnonymousCandidate.FirstName,
                            Email = r.AnonymousCandidate.Email
                        };
                    }

                    if (r.TestResult != null)
                    {
                        res.TestResult = new TestResultResponse
                        {
                            IsPassed = r.TestResult.IsPassed,
                            TestResultLink = r.TestResult.TestResultLink,
                            TestTemplateStatisticstLink = r.TestResult.TestTemplateStatisticsLink
                        };
                    }

                    return res;
                })
            };

            return Ok(result);
        }


        /// <summary>
        /// Удалить отклик
        /// </summary>
        [HttpDelete]
        [Route("response/delete")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> DeleteResponse(int id)
        {
            string? link = _linkRepository.GetTestLinkResponseRowTestResultLink(id);
            if(link != null)
            {
                _testDocumentRepository.DeleteTestResult(link);
            }
            _linkRepository.DeleteTestLinkResponse(id);
            return Ok();
        }



        /// <summary>
        /// Получить результат прохождения теста
        /// </summary>
        [HttpGet]
        [Route("response/result")]
        [SwaggerResponse(200, "Успешный запрос", typeof(Domain.TestResultDocument.TestResult))]
        [SwaggerResponse(400, "Результат не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetTestResult(string link)
        {
            Domain.TestResultDocument.TestResult? result = await _testDocumentRepository.GetTestResult(link);

            if (result != null) return Ok(result);

            return BadRequest("Результат не найден");
        }


        /// <summary>
        /// Получить статистику шаблона по пройденному тесту
        /// </summary>
        /// <remarks>
        /// ТИП ВОЗРАЩАЕМОГО ЗНАЧЕНИЯ СМОТРЕТЬ
        /// В FAG test_template_statistics.json ФАЙЛЕ.
        /// </remarks>
        [HttpGet]
        [Route("response/template-statistics")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Результат не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetTestTemplateStatistics(string link)
        {
            TestApi.Domain.TestTemplateStatisticsDocument.TemplateStatistics? stat =
                await _testDocumentRepository.GetTestTemplateStatistics(link);

            if (stat != null) return Ok(stat);

            return BadRequest("Результат не найден");
        }
    }

}
