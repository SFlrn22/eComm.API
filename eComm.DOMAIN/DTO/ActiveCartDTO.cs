using eComm.DOMAIN.Models;

namespace eComm.DOMAIN.DTO
{
    public class ActiveCartDTO
    {
        public List<CartProduct> Products { get; set; } = new List<CartProduct>();
        public int TotalAmount { get; set; } = default!;
    }
}
