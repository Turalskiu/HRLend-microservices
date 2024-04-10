using Newtonsoft.Json;

namespace Assistant.Api.Domain.Gpt
{
    public class Option
    {
        [JsonProperty("is_true")]
        public bool IsTrue { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Question
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("options")]
        public List<Option> Options { get; set; }
    }

    public class QuestionRoot
    {
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }
}
