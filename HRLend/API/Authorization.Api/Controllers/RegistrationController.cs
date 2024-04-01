using Helpers.Session;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Session;
using Contracts.Authorization.Queue;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using AuthorizationApi.Services.Queue;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthorizationApi.Controllers
{

    [ApiController]
    [Route("auth/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private ICabinetRepository _cabinetRepository;
        private IUserRepository _userRepository;
        private IUserService _userService;
        private IMailService _mailService;
        private IAuthPublisherService _authPublisherService;

        public RegistrationController(
            ICabinetRepository cabinetRepository,
            IUserRepository userRepository,
            IUserService userService,
            IMailService mailService,
            IAuthPublisherService authPublisherService
            )
        {
            _cabinetRepository = cabinetRepository;
            _userRepository = userRepository;
            _userService = userService;
            _mailService = mailService;
            _authPublisherService = authPublisherService;
        }


        /// <summary>
        /// Регистрация (1 стадия)
        /// </summary>
        /// <remarks>
        /// На почту отправляется ссылка для подтверждения регистрации
        /// </remarks>
        [HttpPost]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Почта или логин заняты")]
        [SwaggerResponse(404, "Не удалось отправить ссылку на почту")]
        public IActionResult Registration(RegistrationRequest model)
        {
            if(_userRepository.IsExistsUsernameUser(model.Username)) return BadRequest("Логин занят");
            if (_userRepository.IsExistsEmailUser(model.Email)) return BadRequest("Почта занята");

            string? code = _mailService.SendKeyAuthorization(model);

            if (code != null)
            {
                var user = new RegistrationSession
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    CabinetTitle = model.CabinetTitle,
                    KeyActivate = code,
                    DateCreate = DateTime.Now
                };

                ControllerContext.HttpContext.Session.Set<RegistrationSession>("UserAuth", user);
                return Ok(new { message = "Зайдите в почту для активации аккаунта" });
            }
            return NotFound();
        }


        /// <summary>
        /// Регистрация (2 стадия - подтверждение)
        /// </summary>
        /// <remarks>
        /// Подтверждение регистрации. Создается кабинет, пользователю
        /// выдаются права администратора кабинета.
        /// </remarks>
        [HttpPost("{key}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верный код")]
        [SwaggerResponse(500, "Не удалось зарегестрироваться")]
        public IActionResult Registration(string key)
        {
            var user = ControllerContext.HttpContext.Session.Get<RegistrationSession>("UserAuth");

            if (user.KeyActivate.Equals(key))
            {
                User newUser = _userService.Registration(user);
                if (user != null)
                {
                    _authPublisherService.CabinetMessage(new CabinetQM
                    {
                        MessageType = (int)CABINET_MESSAGE_TYPE.ADD,
                        CabinetId = newUser.CabinetId
                    });

                    _authPublisherService.UserMessage(new UserQM
                    {
                        MessageType = (int)USER_MESSAGE_TYPE.ADD,
                        UserId = newUser.Id,
                        Username = user.Username,
                        UserEmail = user.Email,
                        UserPhoto = null
                    });

                    ControllerContext.HttpContext.Session.Remove("UserAuth");
                    return Ok(new { message = "Авторизация пройдена" });
                }
                return StatusCode(500, "Не удалось зарегестрироваться");
            }
            return BadRequest();
        }


        /// <summary>
        /// Регистрация по токену (1 стадия)
        /// </summary>
        /// <remarks>
        /// При переходе на страницу, в url страницы должен присутствовать
        /// токен, который должен быть передан в модели в мести с данными регистрации.
        /// Токен проходит проверку на валидность, и только при успешной проверки, на
        /// почту отправляется ссылка для потдверждения регистрации.
        /// </remarks>
        [HttpPost("invitation")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не действительный или просроченный токен/логин или почта заняты")]
        [SwaggerResponse(404, "Не удалось отправить ссылку на почту")]
        public IActionResult RegistrationByToken(RegistrationByTokenReguest model)
        {
            var registrationToken = _cabinetRepository.GetRegistrationToken(model.Token);

            if (registrationToken != null)
            {
                if(DateTime.Now > registrationToken.Expires)
                    return BadRequest("Время жизни токена истек");

                if (_userRepository.IsExistsUsernameUser(model.Username)) return BadRequest("Логин занят");
                if (_userRepository.IsExistsEmailUser(model.Email)) return BadRequest("Почта занята");

                string? code = _mailService.SendKeyAuthorization(new RegistrationRequest
                {
                    Username = model.Username,
                    Email = model.Email,
                    PageСonfirmationLink = model.PageСonfirmationLink
                });

                if(code!= null)
                {
                    var user = new RegistrationByTokenSession
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password,
                        CabinetId = registrationToken.Cabinet,
                        CabinetRoleId = registrationToken.CabinetRole,
                        KeyActivate = code,
                        DateCreate = DateTime.Now
                    };

                    _cabinetRepository.DeleteRegistrationToken(registrationToken.Id);
                    ControllerContext.HttpContext.Session.Set<RegistrationByTokenSession>("UserAuthByToken", user);
                    return Ok(new { message = "Зайдите в почту для активации аккаунта" });
                }
                return NotFound();
            }
            return BadRequest("Токен не действителен");
        }


        /// <summary>
        /// Регистрация по токену (2 стадия - подтверждение)
        /// </summary>
        /// <remarks>
        /// Подтверждение регистрации. Пользователь присоединяется
        /// к кабинету, пользователю выдаются права указанные в токене.
        /// </remarks>
        [HttpPost("invitation/{key}")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Ссылка просрочена или не действительна")]
        [SwaggerResponse(500, "Не удалось зарегестрироваться")]
        public IActionResult RegistrationByToken(string key)
        {
            var user = ControllerContext.HttpContext.Session.Get<RegistrationByTokenSession>("UserAuthByToken");

            if (user.KeyActivate.Equals(key))
            {
                User newUser = _userService.Registration(user);
                if (newUser != null)
                {
                    _authPublisherService.UserMessage(new UserQM
                    {
                        MessageType = (int)USER_MESSAGE_TYPE.ADD,
                        UserId = newUser.Id,
                        Username = user.Username,
                        UserEmail = user.Email,
                        UserPhoto = null
                    });

                    ControllerContext.HttpContext.Session.Remove("UserAuthByToken");
                    return Ok(new { message = "Авторизация пройдена" });
                }
                return StatusCode(500, "Не удалось зарегестрироваться");
            }
            return BadRequest();
        }
    }
}
