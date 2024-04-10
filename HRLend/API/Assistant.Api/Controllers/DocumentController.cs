using Assistant.Api.Domain;
using Assistant.Api.Repository;
using AssistantApi.Domain.Auth;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AssistantApi.Attributes;
using Assistant.Api.Services;
using Assistant.Api.Domain.DTO.Request;
using Assistant.Api.Domain.Elasticsearch;


namespace Assistant.Api.Controllers
{
    [Route("assistant/[controller]")]
    [Authorize(Role = "cabinet_hr")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IElasticsearchRepository _elasticsearchRepository;
        private readonly ISplitDocumentService _splitDocumentService;

        private int _splitSize = 1024;

        public DocumentController(
            IDocumentRepository documentRepository,
            IElasticsearchRepository elasticsearchRepository,
            ISplitDocumentService splitDocumentService)
        {
            _documentRepository = documentRepository;
            _elasticsearchRepository = elasticsearchRepository;
            _splitDocumentService = splitDocumentService;
        }


        /// <summary>
        /// Получить список документов
        /// </summary>
        [HttpGet]
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
        /// Добавить документ
        /// </summary>
        [HttpPost]
        [Route("add")]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Ошибка сервера, не получилось создать тест")]
        public async Task<ActionResult> DocumentAdd(DocumentAddRequest doc)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;

            List<string> bb = _splitDocumentService.SplitDocument(doc.Text, _splitSize);
            List<Block> blocks = new List<Block>();

            string documentIndex = Guid.NewGuid().ToString();

            for(int i = 0; i < bb.Count; i++)
            {
                blocks.Add(new Block
                {
                    Content = bb[i],
                    DocumentId = documentIndex
                });
            }

            await _elasticsearchRepository.InsertManyBlock(blocks);

            _documentRepository.InsertDocument(new Document
            {
                CabinetId = cabinetId,
                Title = doc.Title,
                ElasticsearchIndex = documentIndex
            });

            return Ok();
        }


        /// <summary>
        /// Удаление документа
        /// </summary>
        [HttpDelete()]
        [SwaggerResponse(200, "Успешный запрос")]
        [SwaggerResponse(400, "Не верные данные")]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> DocumentDelete(int id)
        {
            Document? doc = _documentRepository.GetDocument(id);

            if(doc != null)
            {
                await _elasticsearchRepository.DeleteBlock(doc.ElasticsearchIndex);
                _documentRepository.DeleteDocument(id);
            }

            return Ok();
        }
    }
}
