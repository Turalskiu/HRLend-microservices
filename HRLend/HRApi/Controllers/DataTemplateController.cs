using HRApi.Attributes;
using HRApi.Domain;
using HRApi.Domain.Auth;
using CD_GRPC = HRApi.Domain.GRPC.CopyingDataGRPC;
using HRApi.Domain.DTO;
using HRApi.Domain.DTO.Request.TestTemplate;
using HRApi.Domain.DTO.Response.Competence.ForTestTemplate;
using HRApi.Domain.DTO.Response.Skill.ForCompetence;
using HRApi.Domain.DTO.Response.TestTemplate;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using HRApi.Repository.SqlDB;
using HRApi.Repository.gRPC;
using HRApi.Domain.DTO.Request.Competence;
using System.Text.Json;

namespace HRApi.Controllers
{
    [Route("hr/data-template")]
    [Authorize(Role = "cabinet_hr")]
    [ApiController]
    public class DataTemplateController : ControllerBase
    {
        private ITestTemplateRepository _templateRepository;
        private IKnowledgeBaseRepository _knowledgeBaseRepository;


        public DataTemplateController(
            ITestTemplateRepository templateRepository,
            IKnowledgeBaseRepository knowledgeBaseRepository
            )
        {
            _templateRepository = templateRepository;
            _knowledgeBaseRepository = knowledgeBaseRepository;
        }


        /// <summary>
        /// Создать новый шаблон
        /// </summary>
        [HttpPost]
        [Route("template/add")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, шаблон с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult TemplateAdd(TestTemplateAddRequest template)
        {
            try
            {
                var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

                return Ok(_templateRepository.InsertTestTemplate(new TestTemplate
                {
                    Title = template.Title,
                    CabinetId = cabinetId
                },
                    template.CompetenceIds,
                    template.CompetenceNeedIds)
                );
            }
            catch (PostgresException pex)
            {
                if (pex.ErrorCode == 23505)
                    return BadRequest("В кабинете уже существует шаблон с таким названием");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Обнавление шаблона и его структуры
        /// </summary>
        /// <remarks>
        /// Метод обновляет шаблон, все еого связи с компетенциями,
        /// компетенции, все связи компетенции с навыками и навыки 
        /// </remarks>
        [HttpPut]
        [Route("template-constructor/update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, компетенция с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult TemplateConstructorUpdate(TemplateConstructorRequest template)
        {
            string json = JsonSerializer.Serialize(template);
            _templateRepository.UpdateTestTemplateConstructor(json);
            return Ok();
        }


        /// <summary>
        /// Обнавление шаблона
        /// </summary>
        /// <remarks>
        /// Метод удалит все имеющинся связи шаблона с компетенциями
        /// и установит новые. Если список CompetenceIds передать пустым,
        /// метод просто удалит все связи, если присвоить значение null списку,
        /// метод не будет изменять связи шаблона (оставит все как есть).
        /// </remarks>
        [HttpPut]
        [Route("template/update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, шаблон с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult TemplateUpdate(TestTemplateUpdateRequest template)
        {
            try
            {
                _templateRepository.UpdateTestTemplate(new TestTemplate
                {
                    Id = template.Id,
                    Title = template.Title
                },
                    template.CompetenceIds,
                    template.CompetenceNeedIds
                 );
                return Ok();
            }
            catch (PostgresException pex)
            {
                if (pex.ErrorCode == 23505)
                    return BadRequest("В кабинете уже существует шаблон с таким названием");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Удаление шаблона
        /// </summary>
        [HttpDelete]
        [Route("template/delete")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult TemplateDelete(int id)
        {
            try
            {
                _templateRepository.DeleteTestTemplate(id);
                return Ok();
            }
            catch (PostgresException pex)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Получить список всех шаблонов для теста (постраничный вывод)
        /// </summary>
        [HttpGet("templates/page")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListTestTemplateResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetTestTemplates(int page_numb, int page_size, string sort)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            var templates = _templateRepository.SelectTestTemplate(new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            },
                cabinetId
            );

            return Ok(new ListTestTemplateResponse
            {
                TotalRows = templates.TotalRows,
                PageNumber = templates.PageNo,
                PageSize = templates.PageSize,
                Sort = templates.Sort,
                Templates = templates.Select(s => new TestTemplateShortResponse
                {
                    Id = s.Id,
                    Title = s.Title
                })
            });
        }


        /// <summary>
        /// Получить полную информацию шаблонов
        /// </summary>
        [HttpGet("template-info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(TestTemplateResponse))]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetTestTemplate(int id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            TestTemplate? template = _templateRepository.GetTestTemplateAndCompetenceAndSkill(id, cabinetId);

            if (template != null)
            {
                return Ok(new TestTemplateResponse
                {
                    Id = template.Id,
                    Title = template.Title,
                    Competencies = template.Competencies.Select(c => new CompetenceShortForTestTemplateResponse
                    {
                        Id = c.Competence.Id,
                        Title = c.Competence.Title,
                        NeedId = c.CompetenceNeed.Id,
                        NeedTitle = c.CompetenceNeed.Title,
                        Skills = c.Competence.Skills.Select(s => new SkillShortForCompetenceResponse
                        {
                            Id = s.Skill.Id,
                            Title = s.Skill.Title,
                            NeedId = s.SkillNeed.Id,
                            NeedTitle = s.SkillNeed.Title
                        })
                    }),
                });
            }

            return NotFound("Элемент не найден");
        }


        /// <summary>
        /// Копировать шаблона из базы знаний в кабинет
        /// </summary>
        [HttpPost("template-copy")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> CopyTemplate(string id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            CD_GRPC.ProfessionCopy? copy = await _knowledgeBaseRepository.GetCopyProfession(id);

            if (copy != null)
            {
                int templateId = _templateRepository.CopyTestTemplate(cabinetId, copy);
                return Ok(templateId);
            }

            return NotFound("Элемент не найден");
        }
    }
}
