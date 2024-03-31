using Microsoft.AspNetCore.Mvc;
using StatisticApi.Attributes;
using StatisticApi.Domain;
using StatisticApi.Domain.DTO.Response;
using StatisticApi.Repository.DocumentDB;
using Swashbuckle.AspNetCore.Annotations;

namespace StatisticApi.Controllers
{
    [Authorize(Role = "cabinet_hr")]
    [ApiController]
    [Route("user-statistic")]
    public class StatisticController : ControllerBase
    {

        private readonly IStatisticRepository _statisticRepository;

        public StatisticController(
            IStatisticRepository statisticRepository
            )
        {
            _statisticRepository = statisticRepository;
        }


        /// <summary>
        /// Получить статистику по id пользователя
        /// </summary>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Успешный запрос", typeof(UserStatisticResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetLink(int id)
        {
            UserStatistic? stat = await _statisticRepository.GetStatistic(id);

            if (stat != null)
            {
                return Ok(new UserStatisticResponse 
                {
                    Skills = stat.Skills,
                    Competencies = stat.Competencies
                });
            }

            return Ok(null);
        }


    }
}
