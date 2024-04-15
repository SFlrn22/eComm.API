namespace eComm.DOMAIN.Models
{
    public class CartProduct
    {
        public int BookID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string ImageUrlS { get; set; } = string.Empty;
        public int Count { get; set; } = default!;
        public int Price { get; set; } = default!;
    }
}
