using HRApi.Domain.GRPC.TestModuleGRPC;

namespace HRApi.Domain.DTO.Request.Skill
{
    public class SkillAddRequest
    {
        public string Title { get; set; }
        public Module? TestModule { get; set; }
    }
}
