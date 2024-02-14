namespace eComm.DOMAIN.Requests
{
    public class GetProductsRequest
    {
        public int PageNumber { get; set; } = default!;
        public int ItemsPerPage { get; set; } = default!;
        public string? SortingColumn { get; set; } = default!;
        public string? SortingType { get; set; } = default!;
    }
}
