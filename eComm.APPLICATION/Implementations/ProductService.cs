using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Responses;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<BaseResponse<Product>> GetProduct(int id)
        {
            _logger.LogCritical($"GetProduct request at {DateTime.Now}");

            BaseResponse<Product> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            try
            {
                Product product = await _productRepository.GetProduct(id);
                response.Data = product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetProduct la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }

        public async Task<BaseResponse<List<ProductDTO>>> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType)
        {
            _logger.LogCritical($"GetProducts request at {DateTime.Now}");

            BaseResponse<List<ProductDTO>> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            try
            {
                List<ProductDTO> products = await _productRepository.GetProducts(pageNumber, itemsPerPage, sortingColumn, sortingType);
                response.Data = products;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetProduct la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }
    }
}
