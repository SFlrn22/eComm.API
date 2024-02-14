using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Responses;

namespace eComm.APPLICATION.Contracts
{
    public interface IProductService
    {
        Task<BaseResponse<List<ProductDTO>>> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType);
        Task<BaseResponse<Product>> GetProduct(int id);
    }
}
