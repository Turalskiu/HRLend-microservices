using HRApi.Domain;
using HRApi.Domain.DTO;
using Helpers.Db.Postgres;
using CD_GRPC = HRApi.Domain.GRPC.CopyingDataGRPC;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;

namespace HRApi.Repository.SqlDB
{
    public interface ITestTemplateRepository
    {
        int InsertTestTemplate(TestTemplate template, int[] competenceIds, int[] competenceNeedIds);
        int CopyTestTemplate(int cabinetId, CD_GRPC.ProfessionCopy copy);
        void UpdateTestTemplate(TestTemplate template, int[] competenceIds, int[] competenceNeedIds);
        void UpdateTestTemplateConstructor(string templateJson);
        void DeleteTestTemplate(int id);
        TestTemplate? GetTestTemplateAndCompetenceAndSkill(int id, int cabinetId);
        IEnumerableWithPage<TestTemplate> SelectTestTemplate(Page page, int cabinetId);
    }

    public class TestTemplateRepository : ITestTemplateRepository
    {
        private readonly string _connectionString;
        public TestTemplateRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertTestTemplate(TestTemplate template, int[] competenceIds, int[] competenceNeedIds)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", template.CabinetId),
                new KeyValuePair<string, object>("@Title", template.Title??string.Empty),
                new KeyValuePair<string, object>("@IdTemplate", 0)
            };

            if (competenceIds != null)
            {
                parames.Add(new KeyValuePair<string, object>("@CompetenceIds", competenceIds));
                parames.Add(new KeyValuePair<string, object>("@CompetenceNeedIds", competenceNeedIds));
            }

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call hr.test_template__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public int CopyTestTemplate(int cabinetId, CD_GRPC.ProfessionCopy copy)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@IdTestTemplate", 0)
            };

            string json = JsonSerializer.Serialize(copy);
            var copyTemplate = new NpgsqlParameter("@TemplateCopyJson", json);
            copyTemplate.NpgsqlDbType = NpgsqlDbType.Jsonb;


            var specialParames = new List<NpgsqlParameter>()
            {
                copyTemplate
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call hr.test_template__copy(" + queryParam + ", @TemplateCopyJson)",
                false,
                parames,
                specialParames
            );
        }
        public void UpdateTestTemplate(TestTemplate template, int[] competenceIds, int[] competenceNeedIds)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", template.Id),
                new KeyValuePair<string, object>("@Title", template.Title??string.Empty)
            };

            if (competenceIds != null)
            {
                parames.Add(new KeyValuePair<string, object>("@CompetenceIds", competenceIds));
                parames.Add(new KeyValuePair<string, object>("@CompetenceNeedIds", competenceNeedIds));
            }

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.test_template__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateTestTemplateConstructor(string templateJson)
        {
            var parames = new List<KeyValuePair<string, object>>(){};

            var templateJsonParam = new NpgsqlParameter("@TemplateJson", templateJson);
            templateJsonParam.NpgsqlDbType = NpgsqlDbType.Jsonb;


            var specialParames = new List<NpgsqlParameter>()
            {
                templateJsonParam
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.template__update_constructor(" + queryParam + "@TemplateJson)",
                parames,
                specialParames
            );
        }
        public void DeleteTestTemplate(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.test_template__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public TestTemplate? GetTestTemplateAndCompetenceAndSkill(int id, int cabinetId)
        {
            var converters = new List<Action<IDataRecord, IList<TestTemplate>>>();

            Action<IDataRecord, IList<TestTemplate>> empty = (record, list) => { };

            Action<IDataRecord, IList<TestTemplate>> convertTestTemplate = (record, list) =>
            {
                var entity = new TestTemplate
                {
                    Id = id,
                    Title = record.Get<string>("title"),
                    Competencies = new List<TestTemplateAndCompetence>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<TestTemplate>> convertCompetence = (record, list) =>
            {
                var entity = new Competence
                {
                    Id = record.Get<int>("competence_id"),
                    Title = record.Get<string>("competence_title"),
                    Skills = new List<CompetenceAndSkill>()
                };

                TestTemplate template = list.FirstOrDefault(x => x.Id == id);
                if (template != null)
                {
                    template.Competencies.Add(new TestTemplateAndCompetence
                    {
                        CompetenceNeed = new CompetenceNeed
                        {
                            Id = record.Get<int>("competence_need_id"),
                            Title = record.Get<string>("competence_need_title")
                        },
                        Competence = entity
                    });
                }
            };

            Action<IDataRecord, IList<TestTemplate>> convertSkill = (record, list) =>
            {
                int? competenceId = record.GetN<int?>("competence_id");
                if (competenceId.HasValue)
                {
                    var entity = new Skill
                    {
                        Id = record.Get<int>("skill_id"),
                        Title = record.Get<string>("skill_title"),
                        TestModuleLink = record.Get<string>("skill_test_module_link")
                    };

                    Competence competence = list
                        .FirstOrDefault(x => x.Id == id)
                        .Competencies.FirstOrDefault(x => x.Competence.Id == competenceId)
                        .Competence;

                    competence.Skills.Add(new CompetenceAndSkill
                    {
                        Skill = entity,
                        SkillNeed = new SkillNeed
                        {
                            Id = record.Get<int>("skill_need_id"),
                            Title = record.Get<string>("skill_need_title")
                        }
                    });
                }
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@CabinetId", cabinetId)
            };

            converters.Add(empty);
            converters.Add(convertTestTemplate);
            converters.Add(convertCompetence);
            converters.Add(convertSkill);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from hr.test_template_and_competence_and_skill__get('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;" +
                "fetch all in ref3;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<TestTemplate> SelectTestTemplate(Page page, int cabinetId)
        {
            var converters = new List<Action<IDataRecord, IList<TestTemplate>>>();

            Action<IDataRecord, IList<TestTemplate>> empty = (record, list) => { };

            Action<IDataRecord, IList<TestTemplate>> convertTestTemplate = (record, list) =>
            {
                var entity = new TestTemplate
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title"),
                };

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@PageOn", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };

            converters.Add(empty);
            converters.Add(convertTestTemplate);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectManyWithPage(
                "select * from hr.test_template__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
    }
}
