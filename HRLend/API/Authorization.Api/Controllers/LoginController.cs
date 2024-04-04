using Helpers.Session;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Response;
using AuthorizationApi.Models.DTO.Response.UserResponse;
using AuthorizationApi.Models.DTO.Session;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthorizationApi.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private IUserService _userService;
        private IMailService _mailService;
        private IUserRepository _userRepository;


        public LoginController(
            IUserService userService,
            IMailService mailService,
            IUserRepository userRepository
            )
        {
            _userService = userService;
            _mailService = mailService;
            _userRepository = userRepository;
        }


        /// <summary>
        /// Зайти в систему
        /// </summary>
        [HttpPost("authenticate")]
        [SwaggerResponse(200, "Успешный запрос", typeof(LoginResponse))]
        [SwaggerResponse(403, "Пользователь заблокирован", typeof(BlockedUserResponse))]
        [SwaggerResponse(400, "Не верный пароль или логин")]
        [SwaggerResponse(404, "Аккаунт удален")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            AuthenticateResponse response;

            try
            {
                response = _userService.Authenticate(model, ipAddress());
            }
            catch (Exception ex)
            {
                return BadRequest("Не верный пароль или логин");
            }

            if (response != null)
            {
                if (!response.IsBlocked)
                {
                    setTokenCookie(response.RefreshToken);
                    return Ok(new LoginResponse
                    {
                        Username = response.Username,
                        Email = response.Email,
                        Image = response.Image,
                        Roles = response.Roles,
                        JwtToken = response.JwtToken
                    });
                }
                return StatusCode(403, new BlockedUserResponse
                {
                    DateBlocked = (DateTime)response.DateBlocked,
                    DateUnblocked = (DateTime)response.DateUnblocked,
                    ReasonBlocked = response.ReasonBlocked,
                });
            }

            return NotFound("Аккаунт удален");
        }


        /// <summary>
        /// Обновить токен доступа
        /// </summary>
        [HttpGet("refresh-token")]
        [SwaggerResponse(200, "Успешный запрос", typeof(LoginResponse))]
        [SwaggerResponse(403, "Пользователь заблокирован", typeof(BlockedUserResponse))]
        [SwaggerResponse(401, "RefreshToken просрочен, пройдите авторизацию")]
        [SwaggerResponse(404, "Не удалось обновить токен")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());

            if (response != null)
            {
                if (!response.IsBlocked)
                {
                    setTokenCookie(response.RefreshToken);
                    return Ok(new LoginResponse
                    {
                        Username = response.Username,
                        Email = response.Email,
                        Image = response.Image,
                        Roles = response.Roles,
                        JwtToken = response.JwtToken
                    });
                }
                return StatusCode(403, new BlockedUserResponse
                {
                    DateBlocked = (DateTime)response.DateBlocked,
                    DateUnblocked = (DateTime)response.DateUnblocked,
                    ReasonBlocked = response.ReasonBlocked,
                });
            }

            return NotFound();
        }


        /// <summary>
        /// Заменить пароль (1 стадия)
        /// </summary>
        /// <remarks>
        /// 1 стадия: новый пароль сохраняется в сессии, на почту
        /// отправляется код для подтверждения пароля.
        /// </remarks>
        [HttpPost("restore-password/send-code")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(404, "Пользователь не найден")]
        public IActionResult SendCodeForRestorePassword(RestorePasswordRequest model)
        {
            var user = _userRepository.GetUser(model.Username);
            if(user == null) return NotFound("Пользователь не найден");

            string code = _mailService.SendCodeForUpdadePassword(user);

            UpdatePasswordSession updatePasswordModel = new UpdatePasswordSession
            {
                UserId = user.Id,
                NewPassword = model.NewPassword,
                Code = code
            };

            ControllerContext.HttpContext.Session.SetWithExpiration<UpdatePasswordSession>("UpdatePasswordModel", updatePasswordModel, TimeSpan.FromMinutes(2));
            return Ok();
        }


        /// <summary>
        /// Заменить пароль (2 стадия)
        /// </summary>
        [HttpPatch("restore/password")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Код просроченный или не верный")]
        public IActionResult UpdatePassword(string code)
        {
            var updatePasswordModel = ControllerContext.HttpContext.Session.GetWithExpiration<UpdatePasswordSession>("UpdatePasswordModel");

            if (updatePasswordModel != null && updatePasswordModel.Code.Equals(code))
            {
                _userRepository.UpdateUserPassword(updatePasswordModel.UserId, BCrypt.Net.BCrypt.HashPassword(updatePasswordModel.NewPassword));
                ControllerContext.HttpContext.Session.Remove("UpdatePasswordModel");

                return Ok();
            }
            return BadRequest("Код прочроченый или не вереный");
        }


        // helper methods
        private void setTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);

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
