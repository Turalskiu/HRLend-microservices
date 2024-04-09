namespace Assistant.Api.Repository
{
    public interface IPromtRepository
    {
        string GetQuestionGeneratorPromt(string title);

    }

    public class PromtRepository : IPromtRepository
    {
        private readonly string _path;

        public PromtRepository(string path)
        {
            _path = path;
        }

        public string GetQuestionGeneratorPromt(string title)
        {
            string path = _path + "\\QuestionGenerator\\" + title + ".txt";
            string promt = File.ReadAllText(path);
            return promt;
        }

    }
}
