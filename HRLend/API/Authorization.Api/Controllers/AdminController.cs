using AuthorizationApi.Attributes;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Models.DTO.Response.CabinetResponse;
using AuthorizationApi.Models.DTO.Response.GroupResponse;
using AuthorizationApi.Models.DTO.Response.RefreshTokenResponse;
using AuthorizationApi.Models.DTO.Response.UserResponse.ForAdmin;
using AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthorizationApi.Controllers
{
    //[Authorize(Role = "admin")]
    [Authorize(Role = "cabinet_admin")]
    [Route("auth/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailService;
        private IUserRepository _userRepository;
        private IAdminRepository _adminRepository;
        private ICabinetRepository _cabinetRepository;

        public AdminController(
            IUserService userService,
            IMailService mailService,
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            ICabinetRepository cabinetRepository
            )
        {
            _userService = userService;
            _mailService = mailService;
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _cabinetRepository = cabinetRepository;
        }


        /// <summary>
        /// Получить список токенов пользователя
        /// </summary>
        [HttpGet("user/{id}/refresh-tokens")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListRefreshTokenResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetRefreshTokensPage(int id, int page_numb, int page_size, string sort)
        {
            var tokens = _adminRepository.SelectRefreshTokenByUserId(id, new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListRefreshTokenResponse
            {
                TotalRows = tokens.TotalRows,
                PageNumber = tokens.PageNo,
                PageSize = tokens.PageSize,
                Sort = tokens.Sort,
                Tokens = tokens.Select(t => new RefreshTokenResponse
                {
                    Id = t.Id,
                    Token = t.Token,
                    Expires = t.Expires,
                    Created = t.Created,
                    CreatedByIp = t.CreatedByIp,
                    Revoked = t.Revoked,
                    RevokedByIp = t.RevokedByIp,
                    ReplacedByToken = t.ReplacedByToken,
                    ReasonRevoked = t.ReasonRevoked
                })
            });
        }


        /// <summary>
        /// Отправить сообщение пользователю
        /// </summary>
        [HttpPost("send/message")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult SendMessage(MessageRequest message)
        {
            _mailService.SendMessage(message);
            return Ok();
        }


        /// <summary>
        /// Получить список кабинетов
        /// </summary>
        [HttpGet("cabinets")]
        [SwaggerResponse(200, "Успешный запрос", typeof(CabinetShortResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetCabinetsPage(int page_numb, int page_size, string sort)
        {
            var cab = _adminRepository.SelectCabinets(new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListCabinetResponse
            {
                TotalRows = cab.TotalRows,
                PageNumber = cab.PageNo,
                PageSize = cab.PageSize,
                Sort = cab.Sort,
                Cabinets = cab.Select(c => new CabinetShortResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status
                })
            });
        }


        /// <summary>
        /// Получить подробную информацию об кабинете
        /// </summary>
        [HttpGet("cabinet/info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(CabinetResponse))]
        [SwaggerResponse(400, "Кабинет не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetCabinetInfo(int id)
        {
            var cabinet = _cabinetRepository.GetCabinetAndGroupsAndUsers(id);

            if (cabinet != null)
                return Ok(new CabinetResponse
                {
                    Title = cabinet.Title,
                    Description = cabinet.Description,
                    DateCreate = cabinet.DateCreate,
                    DateDelete = cabinet.DateDelete,
                    Status = cabinet.Status,
                    Users = cabinet.Users.Select(u => new UserShortForCabinetResponse
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        Photo = u.Photo
                    }),
                    Groups = cabinet.Groups.Select(g => new GroupResponse
                    {
                        Id = g.Id,
                        Title = g.Title,
                        Type = g.Type
                    })
                });

            return BadRequest("Кабинет не найден");
        }


        /// <summary>
        /// Получить список пользователей
        /// </summary>
        [HttpGet("users")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListUserForAdminResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetUsersPage(int page_numb, int page_size, string sort)
        {
            var users = _adminRepository.SelectUsers(new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListUserForAdminResponse
            {
                TotalRows = users.TotalRows,
                PageNumber = users.PageNo,
                PageSize = users.PageSize,
                Sort = users.Sort,
                Users = users.Select(u => new UserShortForAdminResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Photo = u.Photo,
                    Roles = u.Roles,
                    Status = u.Status
                })
            });
        }


        /// <summary>
        /// Получить подробную информацию об пользователи
        /// </summary>
        [HttpGet("user/info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(UserForAdminResponse))]
        [SwaggerResponse(400, "Не верные данные, пользователь не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult GetUserInfo(int id)
        {

            var userInfo = _userRepository.GetUserInfo(id);

            if (userInfo != null)
            {
                return Ok(new UserForAdminResponse
                {
                    Id = id, 
                    CabinetId = userInfo.CabinetId,
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

            return BadRequest();
        }


        /// <summary>
        /// Отозвать токены доступа пользователя
        /// </summary>
        [HttpDelete("revoke-token/user")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Токен отсутствует")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            // accept refresh token in request body or cookie
            var token = model.Token;

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }


        //[HttpDelete("delete/user")]
        //public IActionResult DeleteUser(int id)
        //{
        //    _adminRepository.DeleteUser(id);
        //    return Ok();
        //}


        /// <summary>
        /// Заблокировать пользователя
        /// </summary>
        [HttpDelete("blocked/user")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult BlockedUser(BlockedUserRequest model)
        {
            _adminRepository.BlockedUser(model.UserId, model.ReasonBlocked, model.DateUnblocked);
            return Ok();
        }


        /// <summary>
        /// Разблокировать пользователя
        /// </summary>
        [HttpPatch("unblocked/user")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public IActionResult UnblockedUser(int id)
        {
            _adminRepository.UnblockedUser(id);
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
