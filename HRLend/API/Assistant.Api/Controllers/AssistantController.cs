using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AssistantApi.Attributes;
using Assistant.Api.Repository;
using Assistant.Api.Services;
using Assistant.Api.Domain.Gpt;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using Assistant.Api.Domain.Elasticsearch;


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
        private readonly IGptService _gptService;

        private int _countBlock = 2;

        public AssistantController(
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
        /// Сформировать n вопросов по определенной теме
        /// </summary>
        [HttpGet("generated-question")]
        [SwaggerResponse(200, "Успешный запрос", typeof(QuestionRoot))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> SimpleQuestion(string subject, int count)
        {
            string templatePromt = _promtRepository.GetQuestionGeneratorPromt("Simple");

            string promt = templatePromt
                .Replace("{questionCount}", count.ToString())
                .Replace("{subject}", subject);

            string response = await _gptService.Send(promt);

            //Console.WriteLine(response);

            QuestionRoot questions = JsonConvert.DeserializeObject<QuestionRoot>(response);

            return Ok(questions);
        }



        /// <summary>
        /// Сформировать n оприраясь на определенный документ
        /// </summary>
        [HttpGet("generated-question-by-document")]
        [SwaggerResponse(200, "Успешный запрос", typeof(QuestionRoot))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> QuestionBasedOnDocument(string documentId, string subject, int count)
        {
            string templatePromt = _promtRepository.GetQuestionGeneratorPromt("BasedOnDocument");

            List<Block> blocks = await _elasticsearchRepository.FindBlocks(documentId, subject, 4);

            string promt = templatePromt
                .Replace("{questionCount}", count.ToString());

            for(int i = 0; i < 4; i++)
            {
                if(blocks.Count > i)
                    promt = promt.Replace("{block" + (i+1).ToString() + "}", blocks[i].Content);
                else
                    promt = promt.Replace("{block" + (i + 1).ToString() + "}", "");
            }

            //Console.WriteLine(promt);

            string response = await _gptService.Send(promt);

            //Console.WriteLine(response);

            QuestionRoot questions = JsonConvert.DeserializeObject<QuestionRoot>(response);

            return Ok(questions);
        }



        /// <summary>
        /// Перефразировать вопрос
        /// </summary>
        [HttpPost("paraphrase")]
        [SwaggerResponse(200, "Успешный запрос", typeof(Question))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<IActionResult> Paraphrase(Question question)
        {
            string templatePromt = _promtRepository.GetQuestionGeneratorPromt("Paraphrase");

            string promt = templatePromt
                .Replace("{question}", JsonConvert.SerializeObject(question));

            Console.WriteLine(promt);

            string response = await _gptService.Send(promt);

            Question questionParaphrase = JsonConvert.DeserializeObject<Question>(response);

            return Ok(questionParaphrase);
        }
    }
}
