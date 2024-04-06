using Assistant.Api.Domain;
using Assistant.Api.Repository;
using AssistantApi.Domain.Auth;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AssistantApi.Attributes;


namespace Assistant.Api.Controllers
{
    [Route("assistant/[controller]")]
    [Authorize(Role = "cabinet_hr")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }


        /// <summary>
        /// Получить список документов
        /// </summary>
        [HttpGet()]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Document>))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> GetDocuments()
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            var documents = _documentRepository.SelectDocument(cabinetId);
            return Ok(documents);
        }


        /// <summary>
        /// Удаление документа
        /// </summary>
        [HttpDelete()]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public ActionResult CompetenceDelete(int id)
        {
            _documentRepository.DeleteDocument(id);
            return Ok();
        }
    }
}
