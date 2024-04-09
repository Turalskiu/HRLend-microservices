using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AssistantApi.Attributes;
using Assistant.Api.Repository;
using Assistant.Api.Services;


namespace Assistant.Api.Controllers
{
    [Route("assistant")]
    [Authorize(Role = "cabinet_hr")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IElasticsearchRepository _elasticsearchRepository;
        private readonly IPromtRepository _promtRepository;

        private int _countBlock = 2;

        public AssistantController(
            IDocumentRepository documentRepository,
            IElasticsearchRepository elasticsearchRepository,
            IPromtRepository promtRepository)
        {
            _documentRepository = documentRepository;
            _elasticsearchRepository = elasticsearchRepository;
            _promtRepository = promtRepository;
        }



    }
}
