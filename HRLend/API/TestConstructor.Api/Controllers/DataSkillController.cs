using Grpc.Core;
using HRApi.Attributes;
using HRApi.Domain;
using HRApi.Domain.Auth;
using CD_GRPC = Contracts.KnowledgeBase.GRPC.CopyingData;
using HRApi.Domain.DTO;
using HRApi.Domain.DTO.Request.Skill;
using HRApi.Domain.DTO.Response.Skill;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using HRApi.Repository.SqlDB;
using HRApi.Repository.gRPC;
using HRApi.Domain.DTO.Response.TestModule;
using Contracts.TestGenerator.GRPC.TestModule;


namespace HRApi.Controllers
{
    
    [ApiController]
    [Route("hr/data-skill")]
    [Authorize(Role = "cabinet_hr")]
    public class DataSkillController : ControllerBase
    {
        private ISkillRepository _skillRepository;
        private ITestModuleRepository _testModuleRepository;
        private IKnowledgeBaseRepository _knowledgeBaseRepository;


        public DataSkillController(
            ISkillRepository skillRepository,
            ITestModuleRepository testModuleRepository,
            IKnowledgeBaseRepository knowledgeBaseRepository
            )
        {
            _skillRepository = skillRepository;
            _testModuleRepository = testModuleRepository;
            _knowledgeBaseRepository = knowledgeBaseRepository;
        }


        /// <summary>
        /// Создать новый навык (skill)
        /// </summary>
        [HttpPost]
        [Route("skill/add")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, skill с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Ошибка сервера, не получилось создать тест")]
        public async Task<ActionResult> SkillAdd(SkillAddRequest skill)
        {

            try
            {

                if (skill.TestModule == null) skill.TestModule = new Module
                {
                    Title = skill.Title,
                    Options = new Options(),
                    Rule = new Rule()
                };
                skill.TestModule.Options.IsDefault = false;
                string testModuleLink = await _testModuleRepository.InsertTestModule(skill.TestModule);

                var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

                return Ok(_skillRepository.InsertSkill(new Skill
                {
                    Title = skill.Title,
                    TestModuleLink = testModuleLink,
                    CabinetId = cabinetId
                })
                );
            }
            catch (RpcException ex)
            {
                return StatusCode(500, "Ошибка сервера");
            }
            catch (PostgresException pex)
            {
                if (pex.ErrorCode == 23505)
                    return BadRequest("В кабинете уже существует skill с таким названием");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Обнавление навыка (skill)
        /// </summary>
        [HttpPut]
        [Route("skill/update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные (например, skill с таким название уже существует в кабинете)")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult SkillUpdate(SkillUpdateRequest skill)
        {
            try
            {
                _skillRepository.UpdateSkill(new Skill
                {
                    Id = skill.Id,
                    TestModuleLink = skill.TestModuleLink,
                    Title = skill.Title
                });
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
        /// Удаление навыка
        /// </summary>
        [HttpDelete]
        [Route("skill/delete")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult SkillDelete(int id)
        {
            try
            {
                _skillRepository.DeleteSkill(id);
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
        /// Получить список skill объектов (постраничный вывод)
        /// </summary>
        [HttpGet("skills/page")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListSkillResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetSkills(int page_numb, int page_size, string sort)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            var skills = _skillRepository.SelectSkill(new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            },
                cabinetId
            );


            return Ok(new ListSkillResponse
            {
                TotalRows = skills.TotalRows,
                PageNumber = skills.PageNo,
                PageSize = skills.PageSize,
                Sort = skills.Sort,
                Skills = skills.Select(s => new SkillResponse
                {
                    Id = s.Id,
                    Title = s.Title
                })
            });

        }


        /// <summary>
        /// Получить полную информацию skill объекта
        /// </summary>
        [HttpGet("skill-info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(SkillResponse))]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetSkill(int id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            Skill? skill = _skillRepository.GetSkill(id, cabinetId);

            if (skill != null)
            {
                return Ok(new SkillResponse
                {
                    Id = skill.Id,
                    Title = skill.Title
                });
            }

            return NotFound("Элемент не найден");
        }

        /// <summary>
        /// Копировать skill из базы знаний в кабинет
        /// </summary>
        [HttpPost("skill-copy")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> CopySkill(SkillCopyRequest id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            CD_GRPC.SkillCopy? copy = await _knowledgeBaseRepository.GetCopySkill(id.ProfessionId, id.CompetenceTitle, id.SkillTitle);

            if (copy != null)
            {
                int skillId = _skillRepository.CopySkill(cabinetId, copy);
                return Ok(skillId);
            }

            return NotFound("Элемент не найден");
        }


        /// <summary>
        /// Получить тест модуль по id skill объекта
        /// </summary>
        [HttpGet("test-module")]
        [SwaggerResponse(200, "Успешный запрос", typeof(TestModuleResponse))]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Не удалось получить тест, сервис не отвечает")]
        public async Task<IActionResult> GetTestModule(int id)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            Skill? skill = _skillRepository.GetSkill(id, cabinetId);

            if (skill != null)
            {
                Module module;
                try
                {
                    module = await _testModuleRepository.GetTestModule(skill.TestModuleLink);
                    return Ok(new TestModuleResponse
                    {
                        Id = module.Id,
                        Title = module.Title,
                        Options = new OptionsResponse
                        {
                            IsDefault = module.Options.IsDefault,
                            CountQuestions = module.Options.CountQuestions,
                            TakeQuestions = module.Options.TakeQuestions,
                            LimitDurationInSeconds = module.Options.LimitDurationInSeconds
                        },
                        Rule = new RuleResponse
                        {
                            MinValueForPassed = module.Rule.MinValueForPassed
                        },
                        Recommendations = module.Recommendations.Select(r => new RecommendationResponse
                        {
                            Title = r.Title,
                            Description = r.Description,
                            Link = r.Link
                        }).ToList(),
                        Questions = module.Questions.Select(q => new QuestionResponse
                        {
                            Id = q.Id,
                            Text = q.Text,
                            Type = q.Type,
                            Description = q.Description,
                            MaxValue = q.MaxValue,
                            Options = q.Options.Select(o => new OptionResponse
                            {
                                Id = o.Id,
                                Text = o.Text,
                                IsTrue = o.IsTrue
                            }).ToList()
                        }).ToList()
                    });
                }
                catch (RpcException ex)
                {
                    // Проверяем, является ли статус "Cancelled"
                    if (ex.Status.StatusCode == Grpc.Core.StatusCode.Cancelled)
                    {
                        return NotFound("Элемент не найден");
                    }
                    else
                    {
                        return StatusCode(500, "Ошибка сервера");
                    }
                }
            }

            return NotFound("Элемент не найден");
        }


        /// <summary>
        /// Обновить тест модуль
        /// </summary>
        [HttpPut("test-module/update")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Элемент не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Не удалось обновить тест, сервис не отвечает")]
        public async Task<IActionResult> UpdateTestModule(Module module)
        {
            try
            {
                module.Options.IsDefault = false;
                if (await _testModuleRepository.UpdateTestModule(module))
                    return Ok();

            }
            catch (RpcException ex)
            {
                // Проверяем, является ли статус "Cancelled"
                if (ex.Status.StatusCode == Grpc.Core.StatusCode.Cancelled)
                {
                    return NotFound("Элемент не найден");
                }
                else
                {
                    return StatusCode(500, "Ошибка сервера");
                }
            }

            return NotFound("Элемент не найден");
        }
    }
}
