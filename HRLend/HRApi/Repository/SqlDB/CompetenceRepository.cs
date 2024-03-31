using HRApi.Domain;
using HRApi.Domain.DTO;
using Helpers.Db.Postgres;
using CD_GRPC = HRApi.Domain.GRPC.CopyingDataGRPC;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Text.Json;

namespace HRApi.Repository.SqlDB
{
    public interface ICompetenceRepository
    {
        int InsertCompetence(Competence competence, int[] skillIds, int[] skillNeedIds);
        int CopyCompetence(int cabinetId, CD_GRPC.CompetenceCopy copy);
        void UpdateCompetence(Competence competence, int[] skillIds, int[] skillNeedIds);
        void UpdateCompetenceConstructor(string competenceJson);
        void DeleteCompetence(int id);
        Competence? GetCompetenceAndSkill(int id, int cabinetId);
        IEnumerableWithPage<Competence> SelectCompetence(Page page, int cabinetId);
        IEnumerable<Competence> SelectCompetenceAndSkill(int cabinetId);
    }

    public class CompetenceRepository : ICompetenceRepository
    {
        private readonly string _connectionString;
        public CompetenceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public int InsertCompetence(Competence competence, int[] skillIds, int[] skillNeedIds)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", competence.CabinetId),
                new KeyValuePair<string, object>("@Title", competence.Title??string.Empty),
                new KeyValuePair<string, object>("@IdCompetence", 0)
            };

            if (skillIds != null)
            {
                parames.Add(new KeyValuePair<string, object>("@SkillIds", skillIds));
                parames.Add(new KeyValuePair<string, object>("@SkillNeedIds", skillNeedIds));
            }

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call hr.competence__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public int CopyCompetence(int cabinetId, CD_GRPC.CompetenceCopy copy)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@IdCompetence", 0)
            };

            string json = JsonSerializer.Serialize(copy);
            var copyCompetence = new NpgsqlParameter("@CompetenceCopyJson", json);
            copyCompetence.NpgsqlDbType = NpgsqlDbType.Jsonb;

            var specialParames = new List<NpgsqlParameter>()
            {
                copyCompetence
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call hr.competence__copy(" + queryParam + ", @CompetenceCopyJson)",
                false,
                parames,
                specialParames
            );
        }
        public void UpdateCompetence(Competence competence, int[] skillIds, int[] skillNeedIds)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", competence.Id),
                new KeyValuePair<string, object>("@Title", competence.Title??string.Empty)
            };

            if (skillIds != null)
            {
                parames.Add(new KeyValuePair<string, object>("@SkillIds", skillIds));
                parames.Add(new KeyValuePair<string, object>("@SkillNeedIds", skillNeedIds));
            }

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.competence__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateCompetenceConstructor(string competenceJson)
        {
            var parames = new List<KeyValuePair<string, object>>() { };

            var competenceJsonParam = new NpgsqlParameter("@CompetenceJson", competenceJson);
            competenceJsonParam.NpgsqlDbType = NpgsqlDbType.Jsonb;


            var specialParames = new List<NpgsqlParameter>()
            {
                competenceJsonParam
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.competence__update_constructor(" + queryParam + "@CompetenceJson)",
                parames,
                specialParames
            );
        }
        public void DeleteCompetence(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call hr.competence__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public Competence? GetCompetenceAndSkill(int id, int cabinetId)
        {
            var converters = new List<Action<IDataRecord, IList<Competence>>>();

            Action<IDataRecord, IList<Competence>> empty = (record, list) => { };
            Action<IDataRecord, IList<Competence>> convertCompetence = (record, list) =>
            {
                var entity = new Competence
                {
                    Id = id,
                    Title = record.Get<string>("title"),
                    Skills = new List<CompetenceAndSkill>()
                };
                list.Add(entity);
            };

            Action<IDataRecord, IList<Competence>> convertSkill = (record, list) =>
            {
                var entity = new Skill
                {
                    Id = record.Get<int>("skill_id"),
                    Title = record.Get<string>("skill_title")
                };

                Competence competence = list.FirstOrDefault(x => x.Id == id);
                if (competence != null)
                {
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
            converters.Add(convertCompetence);
            converters.Add(convertSkill);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from hr.competence_and_skill__get('ref1','ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<Competence> SelectCompetence(Page page, int cabinetId)
        {
            var converters = new List<Action<IDataRecord, IList<Competence>>>();

            Action<IDataRecord, IList<Competence>> empty = (record, list) => { };

            Action<IDataRecord, IList<Competence>> convertCompetence = (record, list) =>
            {
                var entity = new Competence
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
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
            converters.Add(convertCompetence);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectManyWithPage(
                "select * from hr.competence__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
        public IEnumerable<Competence> SelectCompetenceAndSkill(int cabinetId)
        {
            var converters = new List<Action<IDataRecord, IList<Competence>>>();

            Action<IDataRecord, IList<Competence>> empty = (record, list) => { };

            Action<IDataRecord, IList<Competence>> convertCompetence = (record, list) =>
            {
                var entity = new Competence
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title"),
                    Skills = new List<CompetenceAndSkill>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<Competence>> convertSkill = (record, list) =>
            {
                var entity = new Skill
                {
                    Id = record.Get<int>("skill_id"),
                    Title = record.Get<string>("skill_title")
                };

                int competenceId = record.Get<int>("competence_id");
                Competence competence = list.FirstOrDefault(x => x.Id == competenceId);
                if (competence != null)
                {
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
                new KeyValuePair<string, object>("@CabinetId", cabinetId)
            };

            converters.Add(empty);
            converters.Add(convertCompetence);
            converters.Add(convertSkill);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from hr.competence__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
    }

}

