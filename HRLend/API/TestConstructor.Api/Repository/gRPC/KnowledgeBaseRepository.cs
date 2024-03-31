using Grpc.Net.Client;
using HRApi.Domain.GRPC.CopyingDataGRPC;


namespace HRApi.Repository.gRPC
{
    public interface IKnowledgeBaseRepository
    {
        Task<ProfessionCopy> GetCopyProfession(string id);
        Task<CompetenceCopy> GetCopyCompetence(string professionId, string competenceTitle);
        Task<SkillCopy> GetCopySkill(string professionId, string competenceTitle, string skillTitle);
    }


    public class KnowledgeBaseRepository : IKnowledgeBaseRepository
    {
        private readonly string _connectionString;
        public KnowledgeBaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<ProfessionCopy?> GetCopyProfession(string id)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new CopyingData.CopyingDataClient(channel);

            Id idProfession = new Id { Id_ = id };

            ProfessionCopy profession = await client.CopyProfessionAsync(idProfession);
            return profession;
        }

        public async Task<CompetenceCopy?> GetCopyCompetence(string professionId, string competenceTitle)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new CopyingData.CopyingDataClient(channel);

            CompetenceId idCompetence = new CompetenceId 
            {
                ProfessionId = professionId,
                CompetenceTitle = competenceTitle
            };

            CompetenceCopy competence = await client.CopyCompetenceAsync(idCompetence);
            return competence;
        }

        public async Task<SkillCopy?> GetCopySkill(string professionId, string competenceTitle, string skillTitle)
        {
            using var channel = GrpcChannel.ForAddress(_connectionString);
            var client = new CopyingData.CopyingDataClient(channel);

            SkillId idSkill = new SkillId 
            {
                ProfessionId = professionId,
                CompetenceTitle= competenceTitle,
                SkillTitle = skillTitle
            };

            SkillCopy skill = await client.CopySkillAsync(idSkill);
            return skill;
        }
    }
}
