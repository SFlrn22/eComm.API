using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Responses;

namespace eComm.APPLICATION.Contracts
{
    public interface IProductService
    {
        Task<BaseResponse<ProductPaginationResultDTO>> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType, string? filterColumn, string? filterValue);
        Task<BaseResponse<Product>> GetProduct(int id);
        Task<BaseResponse<List<Product>>> GetProductsByName(string productName);
    }
}
