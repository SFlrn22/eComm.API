using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;

namespace eComm.PERSISTENCE.Contracts
{
    public interface IProductRepository
    {
        Task<ProductPaginationResultDTO> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType, string? filterColumn, string? filterValue);
        Task<Product> GetProduct(int id);
        Task<List<ProductDTO>> GetProductsByIsbnList(List<string> isbnList);
        Task<List<Product>> GetProductsByName(string productName);
        Task AddOrRemoveFavorites(AddToFavoriteRequest request);
        Task<List<string>> GetFavoriteProducts(string username);
    }
}
