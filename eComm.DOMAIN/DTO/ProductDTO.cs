namespace eComm.DOMAIN.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; } = default!;
        public string Title { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Main_Cat { get; set; } = string.Empty;
        public decimal Price { get; set; } = default!;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
