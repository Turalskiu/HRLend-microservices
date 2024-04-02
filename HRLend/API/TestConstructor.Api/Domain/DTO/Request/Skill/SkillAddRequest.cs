using Contracts.TestGenerator.GRPC.TestModule;

namespace HRApi.Domain.DTO.Request.Skill
{
    public class SkillAddRequest
    {
        public string Title { get; set; }
        public Module? TestModule { get; set; }
    }
}
