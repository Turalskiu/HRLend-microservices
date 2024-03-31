namespace AuthorizationApi.Models.DTO.Response.CabinetResponse
{
    public class CabinetShortResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public CabinetStatus Status { get; set; }

    }
}
