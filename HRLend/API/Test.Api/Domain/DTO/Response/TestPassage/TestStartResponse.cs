

namespace TestApi.Domain.DTO.Response.TestPassage
{
    public class TestStartResponse
    {
        public int ResponseId { get; set; }
        public TestResultSetingsResponse ResultSetings { get; set; }
        public Contracts.TestGenerator.GRPC.TestGenerator.Test Test {  get; set; }
    }
}
