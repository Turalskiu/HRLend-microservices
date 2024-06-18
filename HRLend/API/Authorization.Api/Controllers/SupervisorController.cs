using Authorization.Api.Models.DTO.Response.RegistrationTokenResponse;
using AuthorizationApi.Attributes;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Response.RegistrationTokenResponse;
using AuthorizationApi.Models.DTO.Session;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthorizationApi.Controllers
{
    [Authorize(Role = "cabinet_admin")]
    [Route("auth/[controller]")]
    [ApiController]
    public class SupervisorController : ControllerBase
    {
        private IMailService _mailService;
        private ICabinetRepository _cabinetRepository;

        public SupervisorController(
            IMailService mailService,
            ICabinetRepository cabinetRepository
            )
        {
            _mailService = mailService;
            _cabinetRepository = cabinetRepository;
        }


        /// <summary>
        /// Обновить информацию кабинета
        /// </summary>
        [HttpPatch("update/cabinet/info")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные, название кабинета занято")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult UpdateCabinet(CabinetRequest model)
        {
            try
            {
                var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
                Cabinet cabinet = new Cabinet
                {
                    Id = userSession.CabinetId,
                    Title = model.Title,
                    Description = model.Description
                };
                _cabinetRepository.UpdateCabinet(cabinet);
                return Ok();
            }
            catch (PostgresException ex)
            {
                if (ex.ErrorCode == 23505)
                    return BadRequest("Название занято");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return BadRequest();
        }


        /// <summary>
        /// Отправить сообщение пользователю кабинета
        /// </summary>
        //[HttpPost("send/message")]
        //[SwaggerResponse(200, "Успешный запрос")]
        //[SwaggerResponse(400, "Не верные данные, не возможно отправить сообщение пользователю")]
        //[SwaggerResponse(401, "Не авторизован")]
        //[SwaggerResponse(403, "Нет прав")]
        //public IActionResult SendMessage(MessageRequest message)
        //{
        //    var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
        //    if (!_cabinetRepository.IsIncludetUserToCabinet(message.Username, userSession.CabinetId))
        //        return BadRequest("Пользователь не принадлежит кабинету");

        //    _mailService.SendMessage(message);
        //    return Ok();
        //}


        /// <summary>
        /// Создать токен для регистрации
        /// </summary>
        /// <remarks>
        /// Администратор кабинета может создать токен регистрации
        /// только с правами hr-ов и сотрудников.
        /// </remarks>
        [HttpPost("create/registration-token")]
        [SwaggerResponse(200, "Успешный запрос", typeof(CreateRegistrationTokenResponse))]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult CreateRegistrationToken(RegistrationTokenRequest token)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            if(token.CabinetRole == (int)ROLE.CABINET_HR || token.CabinetRole == (int)ROLE.CABINET_EMPLOYEE)
            {
                string tt = Guid.NewGuid().ToString();

                bool result = _cabinetRepository.InsertRegistrationToken(new RegistrationToken
                {
                    UserId = userSession.Id,
                    Token = tt,
                    Expires = token.Expires,
                    Cabinet = userSession.CabinetId,
                    CabinetRole = token.CabinetRole,
                    Created = DateTime.Now,
                    CreatedByIp = ipAddress()
                });

                if (result) return Ok(new CreateRegistrationTokenResponse { RegistrationToken = tt});
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
                    CreatedByIp = token.CreatedByIp,
                    Expires = token.Expires,
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


        // helper methods
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
