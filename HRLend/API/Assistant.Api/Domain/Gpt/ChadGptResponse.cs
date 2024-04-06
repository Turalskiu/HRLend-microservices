namespace Assistant.Api.Domain.Gpt
{
    public class ChadGptResponse
    {
        public bool is_success { get; set; }
        public string response { get; set; }
        public int used_words_count { get; set; }
        public int used_tokens_count { get; set; }
    }
}
