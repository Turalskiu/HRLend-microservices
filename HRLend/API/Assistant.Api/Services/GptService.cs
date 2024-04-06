using Assistant.Api.Domain.Gpt;
using OpenAI_API;
using OpenAI_API.Chat;

namespace Assistant.Api.Services
{
    public interface IGptService
    {
        Task<string> Send(string promt);
    }


    public class ChadGptService : IGptService
    {
        public readonly string _url;
        public readonly string _apiKey;

        public ChadGptService(string url, string apiKey)
        {
            _apiKey = apiKey;
            _url = url;
        }

        public async Task<string> Send(string promt)
        {
            using (HttpClient httpClient = new HttpClient())
            {

                // URL, куда будет отправлен POST-запрос
                string url = _url;

                // Данные для отправки (можете заменить на свои данные)
                var request_json = new
                {
                    message = promt,
                    api_key = _apiKey
                };

                // Преобразование данных в формат JSON
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request_json);

                // Создание HTTP Content с JSON-данными
                HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Отправка POST-запроса
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                // Проверка успешности запроса
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ChadGptResponse chadGptResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ChadGptResponse>(responseBody);
                    return chadGptResponse.response;
                }

                return null;
            }
        }
    }

    public class ChatGptService : IGptService
    {
        public readonly string _apiKey;

        public ChatGptService(string url, string apiKey)
        {
            _apiKey = apiKey;
        }


        public async Task<string> Send(string promt)
        {
            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(_apiKey));
            Conversation chat = api.Chat.CreateConversation();
            chat.AppendUserInput(promt);

            string AIChatResponse = await chat.GetResponseFromChatbotAsync();
            return AIChatResponse;
        }
    }
}
