using Swashbuckle.AspNetCore.Annotations;

namespace TestApi.Domain.DTO.Response.TestPassage
{
    public class TestStartResponse
    {
        public int ResponseId { get; set; }
        public TestResultSetingsResponse ResultSetings { get; set; }
        public TestApi.Domain.GRPC.TestGeneratorGRPC.Test Test {  get; set; }
    }
}
