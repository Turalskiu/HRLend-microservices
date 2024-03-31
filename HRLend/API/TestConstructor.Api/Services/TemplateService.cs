using Grpc.Core;
using HRApi.Domain.GRPC.TemplateGRPC;
using HRApi.Repository.SqlDB;



namespace HRApi.Services
{
    public class TemplateService : Template.TemplateBase
    {
        private ITestTemplateRepository _templateRepository;

        public TemplateService(
            ITestTemplateRepository templateRepository
            )
        {
            _templateRepository = templateRepository;
        }


        public override async Task<TestTemplate> CreateTestTemplate(Id id, ServerCallContext context)
        {
            Domain.TestTemplate? template = _templateRepository.GetTestTemplateAndCompetenceAndSkill(id.TemplateId, id.CabinetId);

            if (template != null)
            {
                TestTemplate temp = new TestTemplate
                {
                    Title = template.Title
                };
                temp.Competencies.AddRange(template.Competencies.Select(c =>
                {
                    var result = new Competence
                    {
                        Title = c.Competence.Title,
                        CompetenceNeed = c.CompetenceNeed.Id
                    };
                    result.Skills.AddRange(c.Competence.Skills.Select(s => new Skill
                    {
                        Title = s.Skill.Title,
                        TestModuleLink = s.Skill.TestModuleLink,
                        SkillNeed = s.SkillNeed.Id
                    }));
                    return result;
                }));

                return await Task.FromResult(temp);
            }

            throw new RpcException(new Status(StatusCode.NotFound, "Шаблон не найден"));
        }
        
    }
}
