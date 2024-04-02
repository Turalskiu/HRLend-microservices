
namespace Contracts.TestGenerator.GRPC.TestGenerator
{
    public partial class Test
    {
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
