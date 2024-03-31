using HRApi.Attributes;
using HRApi.Domain;
using HRApi.Domain.Auth;
using CD_GRPC = HRApi.Domain.GRPC.CopyingDataGRPC;
using HRApi.Domain.DTO;
using HRApi.Domain.DTO.Request.Competence;
using HRApi.Domain.DTO.Response.Competence;
using HRApi.Domain.DTO.Response.Skill.ForCompetence;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using HRApi.Repository.SqlDB;
using HRApi.Repository.gRPC;
using System.Text.Json;

namespace HRApi.Controllers
{
    
    [ApiController]
    [Route("hr/data-competence")]
    [Authorize(Role = "cabinet_hr")]
    public class DataCompetenceController : ControllerBase
    {
        private ICompetenceRepository _competenceRepository;
        private IKnowledgeBaseRepository _knowledgeBaseRepository;


        public DataCompetenceController(
            ICompetenceRepository competenceRepository,
            IKnowledgeBaseRepository knowledgeBaseRepository
            )
        {
            _competenceRepository = competenceRepository;
            _knowledgeBaseRepository = knowledgeBaseRepository;
        }

        /// <summary>
        /// Создать новую компетенцию
        /// </summary>
        [HttpPost]
        [Route("competence/add")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, компетенция с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult CompetenceAdd(CompetenceAddRequest comp)
        {
            try
            {
                var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

                return Ok(_competenceRepository.InsertCompetence(new Competence
                {
                    Title = comp.Title,
                    CabinetId = cabinetId
                },
                    comp.SkillIds,
                    comp.SkillNeedIds)
                );
            }
            catch (PostgresException pex)
            {
                if (pex.ErrorCode == 23505)
                    return BadRequest("В кабинете уже существует компетенция с таким названием");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Обнавление компетенции и его структуры
        /// </summary>
        /// <remarks>
        /// Метод обновляет компетенцию, все еого связи с навыками и сами навыки
        /// </remarks>
        [HttpPut]
        [Route("competence-constructor/update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, компетенция с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult CompetenceConstructorUpdate(CompetenceConstructorRequest comp)
        {
            string json = JsonSerializer.Serialize(comp);
            _competenceRepository.UpdateCompetenceConstructor(json);
            return Ok();
        }


        /// <summary>
        /// Обнавление компетенции
        /// </summary>
        /// <remarks>
        /// Метод удалит все имеющинся связи компетенции со скилами
        /// и установит новые. Если список SkillIds передать пустым,
        /// метод просто удалит все связи, если присвоить значение null списку,
        /// метод не будет изменять связи компетенции (оставит все как есть).
        /// </remarks>
        [HttpPut]
        [Route("competence/update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, компетенция с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult CompetenceUpdate(CompetenceUpdateRequest comp)
        {
            try
            {
                _competenceRepository.UpdateCompetence(new Competence
                {
                    Id = comp.Id,
                    Title = comp.Title
                },
                    comp.SkillIds,
                    comp.SkillNeedIds
                );
                return Ok();
            }
            catch (PostgresException pex)
            {
                if (pex.ErrorCode == 23505)
                    return BadRequest("В кабинете уже существует компетенция с таким названием");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Удаление компетенции
        /// </summary>
        [HttpDelete]
        [Route("competence/delete")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult CompetenceDelete(int id)
        {
            try
            {
                _competenceRepository.DeleteCompetence(id);
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
        /// Получить список competence объектов (постраничный вывод)
        /// </summary>
        [HttpGet("competencies/page")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListCompetenceResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetCompetenciesPage(int page_numb, int page_size, string sort)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            var competencies = _competenceRepository.SelectCompetence(new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            },
                cabinetId
            );


            return Ok(new ListCompetenceResponse
            {
                TotalRows = competencies.TotalRows,
                PageNumber = competencies.PageNo,
                PageSize = competencies.PageSize,
                Sort = competencies.Sort,
                Competencies = competencies.Select(s => new CompetenceShortResponse
                {
                    Id = s.Id,
                    Title = s.Title
                })
            });
        }


        /// <summary>
        /// Получить список всех competence кабинета
        /// </summary>
        [HttpGet("competencies")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<CompetenceResponse>))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetCompetencies()
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            var competencies = _competenceRepository.SelectCompetenceAndSkill(cabinetId);

            List<CompetenceResponse> result = competencies.Select(c=> new CompetenceResponse
            {
                Id=c.Id,
                Title = c.Title,
                Skills = c.Skills.Select(s=>new SkillShortForCompetenceResponse
                {
                    Id = s.Skill.Id,
                    Title = s.Skill.Title,
                    NeedId = s.SkillNeed.Id,
                    NeedTitle = s.SkillNeed.Title
                }).ToList()
            }).ToList();


            return Ok(result);
        }


        /// <summary>
        /// Получить полную информацию competence объекта
        /// </summary>
        [HttpGet("competence-info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(CompetenceResponse))]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetCompetence(int id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            Competence? competence = _competenceRepository.GetCompetenceAndSkill(id, cabinetId);

            if (competence != null)
            {
                return Ok(new CompetenceResponse
                {
                    Id = competence.Id,
                    Title = competence.Title,
                    Skills = competence.Skills.Select(c => new SkillShortForCompetenceResponse
                    {
                        Id = c.Skill.Id,
                        Title = c.Skill.Title,
                        NeedId = c.SkillNeed.Id,
                        NeedTitle = c.SkillNeed.Title
                    }),
                });
            }

            return NotFound("Элемент не найден");
        }


        /// <summary>
        /// Копировать компетенцию из базы знаний в кабинет
        /// </summary>
        [HttpPost("competence-copy")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> CopyCompetence(CompetenceCopyRequest id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            CD_GRPC.CompetenceCopy? copy = await _knowledgeBaseRepository.GetCopyCompetence(id.ProfessionId, id.CompetenceTitle);

            if (copy != null)
            {
                int competenceId = _competenceRepository.CopyCompetence(cabinetId, copy);
                return Ok(competenceId);
            }

            return NotFound("Элемент не найден");
        }
    }
}
