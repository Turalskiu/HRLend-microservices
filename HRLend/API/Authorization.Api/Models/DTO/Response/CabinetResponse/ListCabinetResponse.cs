namespace AuthorizationApi.Models.DTO.Response.CabinetResponse
{
    public class ListCabinetResponse
    {
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public IEnumerable<CabinetShortResponse> Cabinets { get; set; }
    }
}
