using AuthorizationApi.Attributes;
using Helpers.Session;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Response.UserResponse.ForUser;
using AuthorizationApi.Models.DTO.Session;
using Contracts.Authorization.Queue;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using AuthorizationApi.Services.Queue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthorizationApi.Controllers
{

    [Authorize]
    [ApiController]
    [Route("auth/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailService;
        private IUserRepository _userRepository;
        private IObjectStoreRepository _objectStoreRepository;
        private IAuthPublisherService _authPublisherService;

        public UserController(
            IUserService userService,
            IMailService mailService,
            IUserRepository userRepository,
            IObjectStoreRepository objectStoreRepository,
            IAuthPublisherService authPublisherService
            )
        {
            _userService = userService;
            _mailService = mailService;
            _userRepository = userRepository;
            _objectStoreRepository = objectStoreRepository;
            _authPublisherService = authPublisherService;
        }


        /// <summary>
        /// Выйти из системы
        /// </summary>
        [HttpDelete("logout")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult Logout()
        {
            var token = Request.Cookies["refreshToken"];
            _userService.RevokeToken(token, ipAddress());

            Response.Cookies.Delete("refreshToken");
            ControllerContext.HttpContext.Session.Clear();

            return Ok(new { message = "logout" });
        }

        /// <summary>
        /// Получить информацию о текущем пользователи
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, "Успешный запрос", typeof(UserShortForUserResponse))]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetUser()
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var user = _userRepository.GetUser(userSession.Id);

            if (user != null)
            {
                return Ok(new UserShortForUserResponse
                {
                    Username = user.Username,
                    Email = user.Email,
                    Photo = user.Photo,
                    Roles = user.Roles
                });
            }
            return NotFound();
        }

        /// <summary>
        /// Получить подробную информацию о текущем пользователи
        /// </summary>
        [HttpGet("info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(UserForUserResponse))]
        [SwaggerResponse(404, "Пользователь не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetUserInfo()
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var userInfo = _userRepository.GetUserInfo(userSession.Id);

            if (userInfo != null)
            {
                return Ok(new UserForUserResponse
                {
                    Username = userInfo.Username,
                    Email = userInfo.Email,
                    Photo = userInfo.Photo,
                    DateCreate = userInfo.DateCreate,
                    DateActivation = userInfo.DateActivation,
                    DateBlocked = userInfo.DateBlocked,
                    DateDelete = userInfo.DateDelete,
                    DateUnblocked = userInfo.DateUnblocked,
                    ReasonBlocked = userInfo.ReasonBlocked,
                    FirstName = userInfo.Info.FirstName,
                    LastName = userInfo.Info.LastName,
                    MiddleName = userInfo.Info.MiddleName,
                    Age = userInfo.Info.Age,
                    Status = userInfo.Status,
                    Roles = userInfo.Roles
                });
            }
            return NotFound();
        }


        /// <summary>
        /// Обновить данные пользователя
        /// </summary>
        [HttpPatch("update/info")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult UpdateUserInfo(UserInfo info)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            _userRepository.UpdateUserInfo(userSession.Id, info);
            return Ok();
        }


        /// <summary>
        /// Обновить логин пользователя
        /// </summary>
        [HttpPatch("update/username")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Логин занят")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult UpdateUsername(string username)
        {
            try
            {
                var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
                _userRepository.UpdateUserUsername(userSession.Id, username);

                _authPublisherService.UserMessage(new UserQM
                {
                    MessageType = (int)USER_MESSAGE_TYPE.UPDATE_USERNAME,
                    UserId = userSession.Id,
                    Username = username
                });

                return Ok();
            }
            catch(PostgresException ex)
            {
                if (ex.ErrorCode == 23505)
                    return BadRequest("Логин занят");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }


        /// <summary>
        /// Обновить фото пользователя
        /// </summary>
        [HttpPatch("update/photo")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult UpdatePhoto(ImgRequest model)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var user = _userRepository.GetUser(userSession.Id);

            string titleImg;

            if (user.Photo.IsNullOrEmpty())
            {
                titleImg = user.Username + "_photo." + model.Type;
                user.Photo = titleImg;
            }
            else
            {
                titleImg = user.Photo;
                _objectStoreRepository.DeleteImg(titleImg);
            }

            _authPublisherService.UserMessage(new UserQM
            {
                MessageType = (int)USER_MESSAGE_TYPE.UPDATE_USERNAME,
                UserId = userSession.Id,
                UserPhoto = titleImg
            });

            _objectStoreRepository.InsertImg(titleImg, model.Img);
            _userRepository.UpdateUserPhoto(user.Id, titleImg);

            return Ok();
        }


        /// <summary>
        /// Обновить пароль (1 стадия)
        /// </summary>
        /// <remarks>
        /// 1 стадия: новый пароль сохраняется в сессии, на почту
        /// отправляется код для подтверждения пароля.
        /// </remarks>
        [HttpPost("update-password/send-code")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult SendCodeForUpdatePassword(string password)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var user = _userRepository.GetUser(userSession.Id);
            string code = _mailService.SendCodeForUpdadePassword(user);

            UpdatePasswordSession updatePasswordModel = new UpdatePasswordSession
            {
                UserId = user.Id,
                NewPassword = password,
                Code = code
            };

            ControllerContext.HttpContext.Session.SetWithExpiration<UpdatePasswordSession>("UpdatePasswordModel", updatePasswordModel, TimeSpan.FromMinutes(2));
            return Ok();
        }


        /// <summary>
        /// Обновить пароль (2 стадия)
        /// </summary>
        [HttpPatch("update/password")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верный код")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult UpdatePassword(string code)
        {
            var updatePasswordModel = ControllerContext.HttpContext.Session.GetWithExpiration<UpdatePasswordSession>("UpdatePasswordModel");

            if (updatePasswordModel != null && updatePasswordModel.Code.Equals(code))
            {
                _userRepository.UpdateUserPassword(updatePasswordModel.UserId, BCrypt.Net.BCrypt.HashPassword(updatePasswordModel.NewPassword));
                ControllerContext.HttpContext.Session.Remove("UpdatePasswordModel");

                return Ok();
            }
            return BadRequest();
        }


        //[HttpDelete("delete")]
        //public IActionResult Delete()
        //{
        //    var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
        //    _userRepository.DeleteUser(userSession.Id);

        //    var token = Request.Cookies["refreshToken"];
        //    _userService.RevokeToken(token, ipAddress());

        //    Response.Cookies.Delete("refreshToken");
        //    ControllerContext.HttpContext.Session.Clear();

        //    return Ok();
        //}


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
