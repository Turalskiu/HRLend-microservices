using Assistant.Api.Domain.DTO.Request.Gpt;
using Assistant.Api.Domain.DTO.Response.Gpt;
using Assistant.Api.Repository.Elasticsearch;
using Assistant.Api.Repository.Folder;
using Assistant.Api.Repository.SqlDB;
using Assistant.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AssistantApi.Attributes;
using Assistant.Api.Domain.Elasticsearch;
using Assistant.Api.Domain.Gpt;
using Newtonsoft.Json;
using AssistantApi.Domain.Auth;
using Assistant.Api.Domain;

namespace Assistant.Api.Controllers
{
    [Route("assistant/company")]
    [ApiController]
    public class AssistantCompanyController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IElasticsearchRepository _elasticsearchRepository;
        private readonly IPromtRepository _promtRepository;
        private readonly IGptService _gptService;

        private int _countBlock = 4;

        public AssistantCompanyController(
            IDocumentRepository documentRepository,
            IElasticsearchRepository elasticsearchRepository,
            IPromtRepository promtRepository,
            IGptService gptService)
        {
            _documentRepository = documentRepository;
            _elasticsearchRepository = elasticsearchRepository;
            _promtRepository = promtRepository;
            _gptService = gptService;
        }


        /// <summary>
        /// Ответить на вопрос про компанию опираясь на документ
        /// </summary>
        [Authorize]
        [HttpPost("info")]
        [SwaggerResponse(200, "Успешный запрос", typeof(PromtResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> QuestionBasedOnDocument(PromtRequest request)
        {
            var cabinetId = ((User)ControllerContext.HttpContext.Items["User"]).CabinetId;
            var documents = _documentRepository.SelectDocument(cabinetId);

            string? documentId = documents.FirstOrDefault(d => d.Type.Id == (int)DOCUMENT_TYPE.FOR_COMPANY_INFO)?.ElasticsearchIndex;

            List<Block> blocks = await _elasticsearchRepository.FindBlocks(documentId, request.Text, 4);

            string promt = ((string)_promtRepository.GetCompanyPromt("BasedOnDocument"))
                .Replace("{question}", request.Text);

            for (int i = 0; i < 4; i++)
            {
                if (blocks.Count > i)
                    promt = promt.Replace("{block" + (i + 1).ToString() + "}", blocks[i].Content);
                else
                    promt = promt.Replace("{block" + (i + 1).ToString() + "}", "");
            }

            Console.WriteLine(promt);

            string response = await _gptService.Send(promt);

            Console.WriteLine(response);

            PromtResponse result = new PromtResponse
            {
                Text = response,
            };

            return Ok(result);
        }


        /// <summary>
        /// Задать любой вопрос на прямую чату gpt
        /// </summary>
        [Authorize]
        [HttpPost("promt")]
        [SwaggerResponse(200, "Успешный запрос", typeof(PromtResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> Promt(PromtRequest promt)
        {
            string response = await _gptService.Send(promt.Text);
            PromtResponse result = new PromtResponse { Text = response };
            return Ok(result);
        }
    }
}
