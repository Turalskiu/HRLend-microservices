using HRApi.Domain;
using HRApi.Domain.DTO;
using CD_GRPC = Contracts.KnowledgeBase.GRPC.CopyingData;
using Helpers.Db.Postgres;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Text.Json;

namespace HRApi.Repository.SqlDB
{
    public interface ISkillRepository
    {
        int InsertSkill(Skill skill);
        int CopySkill(int cabinetId, CD_GRPC.SkillCopy copy);
        void UpdateSkill(Skill skill);
        void DeleteSkill(int id);
        Skill? GetSkill(int id, int cabinetId);
        IEnumerableWithPage<Skill> SelectSkill(Page page, int cabinetId);
    }

    public class SkillRepository : ISkillRepository
    {
        private readonly string _connectionString;

        public SkillRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertSkill(Skill skill)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", skill.CabinetId),
                new KeyValuePair<string, object>("@Title", skill.Title??string.Empty),
                new KeyValuePair<string, object>("@TestModuleLink", skill.TestModuleLink??string.Empty),
                new KeyValuePair<string, object>("@IdSkill", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call hr.skill__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public int CopySkill(int cabinetId, CD_GRPC.SkillCopy copy)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@IdSkill", 0)
            };

            string json = JsonSerializer.Serialize(copy);
            var copySkill = new NpgsqlParameter("@SkillCopyJson", json);
            copySkill.NpgsqlDbType = NpgsqlDbType.Jsonb;


            var specialParames = new List<NpgsqlParameter>()
            {
                copySkill
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call hr.skill__copy(" + queryParam + ", @SkillCopyJson)",
                false,
                parames,
                specialParames
            );
        }
        public void UpdateSkill(Skill skill)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", skill.Id),
                new KeyValuePair<string, object>("@Title", skill.Title??string.Empty),
                new KeyValuePair<string, object>("@TestModuleLink", skill.TestModuleLink??string.Empty)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.skill__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteSkill(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.skill__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public Skill? GetSkill(int id, int cabinetId)
        {

            Func<IDataRecord, Skill> convertSkill = (record) =>
            {
                var entity = new Skill
                {
                    Id = id,
                    Title = record.Get<string>("title"),
                    TestModuleLink = record.Get<string>("test_module_link")
                };

                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@CabinetId", cabinetId)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelect(
                "select * from hr.skill__get(" + queryParam + ");",
                convertSkill,
                parames
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<Skill> SelectSkill(Page page, int cabinetId)
        {
            var converters = new List<Action<IDataRecord, IList<Skill>>>();

            Action<IDataRecord, IList<Skill>> empty = (record, list) => { };

            Action<IDataRecord, IList<Skill>> convertSkill = (record, list) =>
            {
                var entity = new Skill
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title"),
                    TestModuleLink = record.Get<string>("test_module_link")
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
            converters.Add(convertSkill);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectManyWithPage(
                "select * from hr.skill__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
    }
}
