using AuthorizationApi.Attributes;
using AuthorizationApi.Models.DTO.Session;
using AuthorizationApi.Models;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using Microsoft.AspNetCore.Mvc;
using AuthorizationApi.Models.DTO.Request;
using Swashbuckle.AspNetCore.Annotations;
using AuthorizationApi.Models.DTO.Response.RegistrationTokenResponse;
using AuthorizationApi.Services.Queue;
using Contracts.Authorization.Queue;
using Authorization.Api.Models.DTO.Response.RegistrationTokenResponse;

namespace AuthorizationApi.Controllers
{

    [Authorize(Role = "cabinet_hr")]
    [Route("auth/[controller]")]
    [ApiController]
    public class HRController : ControllerBase
    {
        private IMailService _mailService;
        private ICabinetRepository _cabinetRepository;
        private IUserRepository _userRepository;
        private IAuthPublisherService _authPublisherService;

        public HRController(
            IMailService mailService,
            ICabinetRepository cabinetRepository,
            IUserRepository userRepository,
            IAuthPublisherService authPublisherService
            )
        {
            _mailService = mailService;
            _cabinetRepository = cabinetRepository;
            _userRepository = userRepository;
            _authPublisherService = authPublisherService;
        }


        /// <summary>
        /// Создать группу
        /// </summary>
        [HttpPost("create/group")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult CreateGroup(GroupCreateRequest model)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            Group group = new Group
            {
                CabinetId = userSession.CabinetId,
                Title = model.Title,
                TypeId = model.TypeId
            };

            int groupId = _cabinetRepository.InsertGroup(group);

            _authPublisherService.GroupMessage(new GroupQM
            {
                MessageType = (int)GROUP_MESSAGE_TYPE.ADD,
                GroupId = groupId,
                GroupTitle = model.Title
            });

            return Ok();
        }


        /// <summary>
        /// Обновить данные группы
        /// </summary>
        [HttpPatch("update/group")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult UpdateGroup(GroupUpdateRequest model)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            Group group = new Group
            {
                Id = model.Id,
                CabinetId = userSession.CabinetId,
                Title = model.Title
            };

            _cabinetRepository.UpdateGroup(group);

            _authPublisherService.GroupMessage(new GroupQM
            {
                MessageType = (int)GROUP_MESSAGE_TYPE.UPDATE_TITLE,
                GroupId = model.Id,
                GroupTitle = model.Title
            });

            return Ok();
        }


        /// <summary>
        /// Удалить группу
        /// </summary>
        [HttpDelete("delete/group")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult DeleteGroup(int id)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            _cabinetRepository.DeleteGroup(userSession.CabinetId, id);

            _authPublisherService.GroupMessage(new GroupQM
            {
                MessageType = (int)GROUP_MESSAGE_TYPE.DELETE,
                GroupId = id
            });

            return Ok();
        }


        /// <summary>
        /// Добавить пользователя в группу
        /// </summary>
        [HttpPost("group/{group_id}/add/user/{user_id}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult AddConnectionGroupAndUser(int group_id, int user_id)
        {
            _cabinetRepository.ConnectionGroupAndUser(user_id, group_id);
            return Ok();
        }


        /// <summary>
        /// Удалить пользователя из группы
        /// </summary>
        [HttpDelete("group/{group_id}/delete/user/{user_id}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult DeleteConnectionGroupAndUser(int group_id, int user_id)
        {
            _cabinetRepository.DeleteConnectionGroupAndUser(user_id, group_id);
            return Ok();
        }


        /// <summary>
        /// Отправить сообщение на почту
        /// </summary>
        [HttpPost("send/message")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Нет прав отправлять письмо данному пользователю")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult SendMessage(MessageRequest message)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            var recipient = _userRepository.GetUser(message.Username);

            if (recipient != null)
            {
                if(userSession.CabinetId != recipient.CabinetId)
                    return BadRequest("Пользователь не принадлежит кабинету");
                if(!recipient.Roles.Any(r => r.Id == (int)ROLE.CABINET_CANDIDATE || r.Id == (int)ROLE.CABINET_EMPLOYEE)) 
                    return BadRequest("Вы не можете отправлять письмо на почту данному пользователю");

                _mailService.SendMessage(message);
                return Ok();
            }

            return BadRequest("Пользователь не найден");
        }


        /// <summary>
        /// Отправить сообщение на почту всем членам группы
        /// </summary>
        [HttpPost("send/message/group")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Группа не найдена")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult SendMessageGroup(int group_id, NewsletterRequest newsletter)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            var users = _cabinetRepository.GetGroupAndUsers(userSession.CabinetId, group_id)?.Users;

            if (users != null)
            {
                foreach (var user in users)
                {
                    var message = new MessageRequest
                    {
                        Username = user.Username,
                        Email = user.Email,
                        Subject = newsletter.Subject,
                        Message = newsletter.Message
                    };

                    _mailService.SendMessage(message);
                }
      
                return Ok();
            }

            return BadRequest("Группа не найдена");
        }


        /// <summary>
        /// Создать токен для регистрации
        /// </summary>
        /// <remarks>
        /// HR может создать токен регистрации
        /// только с правами кандидатов и сотрудников.
        /// </remarks>
        [HttpPost("create/registration-token")]
        [SwaggerResponse(200, "Успешный запрос", typeof(CreateRegistrationTokenResponse))]
        [SwaggerResponse(400, "Не верные данные токена")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult CreateRegistrationToken(RegistrationTokenRequest token)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            if (token.CabinetRole == (int)ROLE.CABINET_CANDIDATE || token.CabinetRole == (int)ROLE.CABINET_EMPLOYEE)
            {
                string tt = Guid.NewGuid().ToString();

                bool result = _cabinetRepository.InsertRegistrationToken(new RegistrationToken
                {
                    UserId = userSession.Id,
                    Token = Guid.NewGuid().ToString(),
                    Expires = token.Expires,
                    Cabinet = userSession.CabinetId,
                    CabinetRole = token.CabinetRole,
                    Created = DateTime.Now,
                    CreatedByIp = ipAddress()
                });

                if (result) return Ok(new CreateRegistrationTokenResponse { RegistrationToken = tt });
                return BadRequest();
            }

            return BadRequest("Отсутствуют права на создание токена с данной ролью");
        }


        /// <summary>
        /// Получить подробную информацию об токене регистрации
        /// </summary>
        [HttpGet("registration-token/{id}")]
        [SwaggerResponse(200, "Успешный запрос", typeof(RegistrationTokenResponse))]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetRegistrationToken(int id)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            var token = _cabinetRepository.GetRegistrationToken(id);

            if (token != null && token.UserId == userSession.Id)
            {
                return Ok(new RegistrationTokenResponse
                {
                    Id = id,
                    Token = token.Token,
                    Created = token.Created,
                    Expires = token.Expires,
                    CreatedByIp = token.CreatedByIp,
                    CabinetRole = token.CabinetRole
                });
            }
            return BadRequest("Нет такого токена");
        }



        /// <summary>
        /// Получить список токенов регистрации
        /// </summary>
        [HttpGet("registration-token")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListRegistrationTokenResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetRegistrationTokens(int page_numb, int page_size, string sort)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            var tokens = _cabinetRepository.SelectRegistrationToken(userSession.Id, new Models.DTO.Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListRegistrationTokenResponse
            {
                TotalRows = tokens.TotalRows,
                PageNumber = tokens.PageNo,
                PageSize = tokens.PageSize,
                Sort = tokens.Sort,
                Tokens = tokens.Select(t => new RegistrationTokenResponse
                {
                    Id = t.Id,
                    Token = t.Token,
                    Created = t.Created,
                    Expires = t.Expires,
                    CreatedByIp = t.CreatedByIp,
                    CabinetRole = t.CabinetRole
                })
            });
        }


        /// <summary>
        /// Удалить токен регистрации
        /// </summary>
        [HttpDelete("delete/registration-token/{id}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult DeleteRegistrationToken(int id)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            _cabinetRepository.DeleteRegistrationToken(id);

            return Ok();
        }


        /// <summary>
        /// Удалить список токенов регистрации
        /// </summary>
        [HttpDelete("delete/registration-token")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult DeleteRegistrationToken(ListIdRequest list)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            _cabinetRepository.DeleteRegistrationTokens(list.id);

            return Ok();
        }


        /// <summary>
        /// Сделать кандидата сотрудником
        /// </summary>
        [HttpPut("employ/{candidate_id}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult Employ(int candidate_id)
        {
            _userRepository.DeleteUserRole(candidate_id, (int)ROLE.CABINET_CANDIDATE);
            _userRepository.InsertUserRole(candidate_id, (int)ROLE.CABINET_EMPLOYEE);
            return Ok();
        }


        private string ipAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
