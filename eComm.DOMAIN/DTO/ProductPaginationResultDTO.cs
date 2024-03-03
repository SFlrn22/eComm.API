using eComm.DOMAIN.Models;

namespace eComm.DOMAIN.DTO
{
    public class ProductPaginationResultDTO
    {
        public List<Product> ProductList { get; set; } = new List<Product>();
        public int ProductCount { get; set; }
    }
}
