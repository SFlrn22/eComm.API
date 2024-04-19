namespace eComm.DOMAIN.DTO
{
    public class ProductDTO
    {
        public int BookID { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string PublicationYear { get; set; } = string.Empty;
        public string ImageUrlL { get; set; } = string.Empty;
        public int AverageRating { get; set; }
        public int Price { get; set; }
    }
}
