using AuthorizationApi.Attributes;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO;
using AuthorizationApi.Models.DTO.Response.CabinetResponse;
using AuthorizationApi.Models.DTO.Response.GroupResponse;
using AuthorizationApi.Models.DTO.Response.UserResponse.ForCabinet;
using AuthorizationApi.Models.DTO.Session;
using AuthorizationApi.Repository;
using AuthorizationApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace AuthorizationApi.Controllers
{

    [Authorize]
    [Route("auth/[controller]")]
    [ApiController]
    public class CabinetController : ControllerBase
    {

        private IUserService _userService;
        private IMailService _mailService;
        private IUserRepository _userRepository;
        private ICabinetRepository _cabinetRepository;

        public CabinetController(
            IUserService userService,
            IMailService mailService,
            IUserRepository userRepository,
            ICabinetRepository cabinetRepository
            )
        {
            _userService = userService;
            _mailService = mailService;
            _userRepository = userRepository;
            _cabinetRepository = cabinetRepository;
        }


        /// <summary>
        /// Получить кабинет с пользователями и группами
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, "Успешный запрос", typeof(CabinetResponse))]
        [SwaggerResponse(404, "Кабинет не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetCabinet()
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            Cabinet cabinet = _cabinetRepository.GetCabinetAndGroupsAndUsers(userSession.CabinetId);

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
                        Id =g.Id,
                        Title = g.Title,
                        Type = g.Type
                    })
                });

            return NotFound("Кабинет не найден");
        }

        /// <summary>
        /// Получить кабинет
        /// </summary>
        [HttpGet("info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(CabinetShortResponse))]
        [SwaggerResponse(404, "Кабинет не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetCabinetInfo()
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            Cabinet cabinet = _cabinetRepository.GetCabinet(userSession.CabinetId);

            if (cabinet != null)
                return Ok(new CabinetShortResponse
                {
                    Id = cabinet.Id,
                    Title = cabinet.Title,
                    Description = cabinet.Description,
                    Status = cabinet.Status,
                });

            return NotFound("Кабинет не найден");
        }

        /// <summary>
        /// Получить список пользователей кабинета
        /// </summary>
        [HttpGet("users")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListUserForCabinetResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetUsersPageFromCabinet(int page_numb, int page_size, string sort)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var users = _cabinetRepository.SelectUsers(userSession.CabinetId, new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListUserForCabinetResponse
            {
                TotalRows = users.TotalRows,
                PageNumber = users.PageNo,
                PageSize = users.PageSize,
                Sort = users.Sort,
                Users = users.Select(u => new UserShortAndRoleForCabinetResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Photo = u.Photo,
                    Roles = u.Roles
                })
            });
        }


        /// <summary>
        /// Получить информацию об пользователи кабинета
        /// </summary>
        [HttpGet("user/info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(UserForCabinetResponse))]
        [SwaggerResponse(400, "Пользователь не найден")]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetUserInfo(int user_id)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            User user = _cabinetRepository.GetUserInfo(userSession.CabinetId, user_id);

            if(user!= null)
                return Ok(new UserForCabinetResponse
                {
                    Id=user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Photo = user.Photo,
                    FirstName = user.Info.FirstName,
                    LastName = user.Info.LastName,
                    MiddleName = user.Info.MiddleName,
                    Age = user.Info.Age,
                    Roles = user.Roles
                });

            return BadRequest("Пользователь не найден");
        }


        /// <summary>
        /// Получить список групп текущего пользователя
        /// </summary>
        [HttpGet("user/groups")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListGroupResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetGroupsPageByUser(int page_numb, int page_size, string sort)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];

            var groups = _cabinetRepository.SelectGroupsByUser(userSession.Id, new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListGroupResponse
            {
                TotalRows = groups.TotalRows,
                PageNumber = groups.PageNo,
                PageSize = groups.PageSize,
                Sort = groups.Sort,
                Groups = groups.Select(g => new GroupResponse
                {
                    Id = g.Id,
                    Title = g.Title,
                    Type = g.Type
                })
            });
        }


        /// <summary>
        /// Получить список групп кабинета
        /// </summary>
        [HttpGet("groups")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListGroupResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetGroupsPage(int page_numb, int page_size, string sort)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var groups = _cabinetRepository.SelectGroups(userSession.CabinetId, new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListGroupResponse
            {
                TotalRows = groups.TotalRows,
                PageNumber = groups.PageNo,
                PageSize = groups.PageSize,
                Sort = groups.Sort,
                Groups = groups.Select(g => new GroupResponse
                {
                    Id = g.Id,
                    Title = g.Title,
                    Type = g.Type
                })
            });
        }


        /// <summary>
        /// Получить список пользователей группы
        /// </summary>
        [HttpGet("group/{group_id}/users")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListUserForCabinetResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        public IActionResult GetUsersFromGroup(int group_id, int page_numb, int page_size, string sort)
        {
            var userSession = (UserSession)ControllerContext.HttpContext.Items["User"];
            var users = _cabinetRepository.SelectUsersByGroup(userSession.CabinetId, group_id, new Page
            {
                PageNumber = page_numb,
                PageSize = page_size,
                Sort = sort
            });

            return Ok(new ListUserForCabinetResponse
            {
                TotalRows = users.TotalRows,
                PageNumber = users.PageNo,
                PageSize = users.PageSize,
                Sort = users.Sort,
                Users = users.Select(u => new UserShortAndRoleForCabinetResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Photo = u.Photo,
                    Roles = u.Roles
                })
            });
        }

    }
}
