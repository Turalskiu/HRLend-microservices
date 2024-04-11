
using System.Text.Json.Serialization;

namespace Contracts.TestGenerator.GRPC.TestGenerator
{
    public partial class Test
    {
        [JsonIgnore]
        public List<Question> QuestionsList
        {
            get { return questions_.ToList(); }
            set
            {
                questions_.Clear();
                questions_.Add(value);
            }
        }
    }

    public partial class Question
    {
        [JsonIgnore]
        public List<Option> OptionsList
        {
            get { return options_.ToList(); }
            set
            {
                options_.Clear();
                options_.Add(value);
            }
        }
    }
}
