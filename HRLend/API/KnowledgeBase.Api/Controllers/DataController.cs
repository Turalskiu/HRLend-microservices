using KnowledgeBaseApi.Attributes;
using KnowledgeBaseApi.Domain;
using KnowledgeBaseApi.Repository;
using KnowledgeBaseApi.Repository.DocumentDB;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KnowledgeBaseApi.Controllers
{

    [ApiController]
    [Route("kb/[controller]")]
    [Authorize(Role = "cabinet_hr")]
    public class DataController : ControllerBase
    {

        private IProfessionRepository _professionRepository;


        public DataController(
            IProfessionRepository professionRepository
            )
        {
            _professionRepository = professionRepository;
        }


        /// <summary>
        /// Получить список profession объектов
        /// </summary>
        [HttpGet("professions")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Profession>))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> GetProfessions(int skip, int take, string sort)
        {
            var professions = await _professionRepository.SelectProfession(skip, take, sort);
            return Ok(professions);
        }

        
    }
}
