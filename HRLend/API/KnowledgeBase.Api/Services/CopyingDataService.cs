using Grpc.Core;
using Contracts.KnowledgeBase.GRPC.CopyingData;
using KnowledgeBaseApi.Repository.DocumentDB;



namespace KnowledgeBaseApi.Services
{
    public class CopyingDataService : CopyingData.CopyingDataBase
    {
        private IProfessionRepository _professionRepository;

        public CopyingDataService(
            IProfessionRepository professionRepository
            )
        {
            _professionRepository = professionRepository;
        }


        public override async Task<ProfessionCopy> CopyProfession(Id id, ServerCallContext context)
        {
            Domain.Profession? prof = await _professionRepository.GetProfession(id.Id_);

            if (prof != null)
            {
                ProfessionCopy copy = new ProfessionCopy
                {
                    Title = prof.Title
                };

                copy.Competencies.AddRange(prof.Competencies.Select(c => {
                    var result = new Competence
                    {
                        Title = c.Title,
                        CompetenceNeed = c.CompetenceNeed.RequiredCode,
                    };
                    result.Skills.AddRange(c.Skills.Select(s => new Skill
                    {
                        Title = s.Title,
                        TestModuleLink = s.TestModuleId,
                        SkillNeed = s.SkillNeed.RequiredCode
                    }));
                    return result;
                }));

                return await Task.FromResult(copy);
            }

            throw new RpcException(new Status(StatusCode.NotFound, "Компетенция не найден"));
        }
        public override async Task<CompetenceCopy> CopyCompetence(CompetenceId id, ServerCallContext context)
        {
            Domain.Competence? comp = await _professionRepository.GetCompetence(id.ProfessionId, id.CompetenceTitle);

            if (comp != null)
            {
                CompetenceCopy copy = new CompetenceCopy
                {
                    Title = comp.Title
                };
                copy.Skills.AddRange(comp.Skills.Select(s => new Skill
                {
                    Title = s.Title,
                    TestModuleLink = s.TestModuleId,
                    SkillNeed = s.SkillNeed.RequiredCode
                }));

                return await Task.FromResult(copy);
            }

            throw new RpcException(new Status(StatusCode.NotFound, "Компетенция не найден"));
        }
        public override async Task<SkillCopy> CopySkill(SkillId id, ServerCallContext context)
        {
            Domain.Skill? skill = await _professionRepository.GetSkill(id.ProfessionId, id.CompetenceTitle, id.SkillTitle);

            if (skill != null)
            {
                SkillCopy copy = new SkillCopy
                { 
                    Title = skill.Title,
                    TestModuleLink = skill.TestModuleId
                };
                return await Task.FromResult(copy);
            }
                
            throw new RpcException(new Status(StatusCode.NotFound, "Skill не найден"));
        }
    }
}
