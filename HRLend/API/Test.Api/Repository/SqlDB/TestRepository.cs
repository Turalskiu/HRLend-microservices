using System.Data;
using TestApi.Domain;
using TestApi.Domain.DTO;
using Helpers.Db.Postgres;

namespace TestApi.Repository.SqlDB
{
    public interface ITestRepository
    {
        int InsertTest(Test test);
        void UpdateTest(Test test);
        void DeleteTest(int id);
        Test? GetTest(int id);
        Test? GetTestAndTestLink(int id);
        IEnumerableWithPage<Test> SelectTest(Page page, int hrId);
        string? GetTestRowTestTemplateLink(int id);
    }

    public class TestRepository : ITestRepository
    {
        private readonly string _connectionString;

        public TestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertTest(Test test)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@HrId", test.HrId),
                new KeyValuePair<string, object>("@Title", test.Title??string.Empty),
                new KeyValuePair<string, object>("@Description", test.Description??string.Empty),
                new KeyValuePair<string, object>("@TestTemplateLink", test.TestTemplateLink??string.Empty),
                new KeyValuePair<string, object>("@IdTest", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call test.test__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateTest(Test test)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", test.Id),
                new KeyValuePair<string, object>("@Title", test.Title??string.Empty),
                new KeyValuePair<string, object>("@Description", test.Description??string.Empty)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteTest(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public Test? GetTest(int id)
        {
            var converters = new List<Action<IDataRecord, IList<Test>>>();

            Action<IDataRecord, IList<Test>> empty = (record, list) => { };
            Action<IDataRecord, IList<Test>> convertTest = (record, list) =>
            {
                var entity = new Test
                {
                    Id = id,
                    Title = record.Get<string>("title"),
                    Description = record.GetN<string?>("description"),
                    TestTemplateLink = record.Get<string>("test_template_link"),
                    Links = new List<TestLink>()
                };
                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            converters.Add(empty);
            converters.Add(convertTest);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from test.test__get('ref1'," + queryParam + ");" +
                "fetch all in ref1;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public Test? GetTestAndTestLink(int id)
        {
            var converters = new List<Action<IDataRecord, IList<Test>>>();

            Action<IDataRecord, IList<Test>> empty = (record, list) => { };
            Action<IDataRecord, IList<Test>> convertTest = (record, list) =>
            {
                var entity = new Test
                {
                    Id = id,
                    Title = record.Get<string>("title"),
                    Description = record.GetN<string?>("description"),
                    TestTemplateLink = record.Get<string>("test_template_link"),
                    Links = new List<TestLink>()
                };
                list.Add(entity);
            };

            Action<IDataRecord, IList<Test>> convertTestLink = (record, list) =>
            {
                var entity = new TestLink
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title"),
                    Status = new TestLinkStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    }
                };

                Test test = list.FirstOrDefault(x => x.Id == id);
                if (test != null)
                {
                    test.Links.Add(entity);
                }
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            converters.Add(empty);
            converters.Add(convertTest);
            converters.Add(convertTestLink);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from test.test_and_test_link__get('ref1','ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<Test> SelectTest(Page page, int hrId)
        {
            var converters = new List<Action<IDataRecord, IList<Test>>>();

            Action<IDataRecord, IList<Test>> empty = (record, list) => { };

            Action<IDataRecord, IList<Test>> convertTest = (record, list) =>
            {
                var entity = new Test
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
                };

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@HrId", hrId),
                new KeyValuePair<string, object>("@PageOn", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };

            converters.Add(empty);
            converters.Add(convertTest);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectManyWithPage(
                "select * from test.test__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
        public string? GetTestRowTestTemplateLink(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            var result = _connectionString.ExecuteScalar<string>(
                "select * from test.test__get_test_template_link(" + queryParam + ")",
                false,
                parames
            );

            if (result == "") return null;
            return result;
        }
    }
}
