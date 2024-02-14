namespace eComm.DOMAIN.Models
{
    public class Product
    {
        public int Id { get; set; } = default!;
        public string Title { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Main_Cat { get; set; } = string.Empty;
        public decimal Price { get; set; } = default!;
        public string Asin { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageUrlHighRes { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
