using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;

namespace eComm.PERSISTENCE.Contracts
{
    public interface IProductRepository
    {
        Task<ProductPaginationResultDTO> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType);
        Task<Product> GetProduct(int id);
    }
}
