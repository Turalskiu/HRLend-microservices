namespace Assistant.Api.Repository.Folder
{
    public interface IPromtRepository
    {
        string GetQuestionGeneratorPromt(string title);
        string GetCompanyPromt(string title);

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
            string path = _path + "/Promts/QuestionGenerator/" + title + ".txt";
            string promt = File.ReadAllText(path);
            return promt;
        }

        public string GetCompanyPromt(string title)
        {
            string path = _path + "/Promts/CompanyInfo/" + title + ".txt";
            string promt = File.ReadAllText(path);
            return promt;
        }
    }
}
