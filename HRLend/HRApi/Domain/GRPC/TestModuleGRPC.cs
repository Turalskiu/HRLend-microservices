
namespace HRApi.Domain.GRPC.TestModuleGRPC
{
    public partial class Module
    {
        // Добавьте явные свойства с правамиset для списка questions   
        public List<Question> QuestionList
        {
            get { return questions_.ToList(); }
            set
            {
                questions_.Clear();
                questions_.Add(value);
            }
        }

        public List<Recommendation> RecommendationsList
        {
            get { return recommendations_.ToList(); }
            set
            {
                recommendations_.Clear();
                recommendations_.Add(value);
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
