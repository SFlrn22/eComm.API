using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IShareService _shareService;
        private readonly IScrapperService _scrapperService;
        private readonly ILogRepository _logRepository;
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IShareService shareService, IScrapperService scrapperService, ILogRepository logRepository)
        {
            _productRepository = productRepository;
            _logger = logger;
            _shareService = shareService;
            _scrapperService = scrapperService;
            _logRepository = logRepository;
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
                if (product.Description == null || product.Category == null || product.Price == 0)
                {
                    var prices = _scrapperService.GetPriceFromAmazon(product.ISBN);
                    ScrappedData data = await _scrapperService.GetCatAndDesc(product.ISBN);
                    product.Price = Convert.ToInt32(prices[0]);
                    product.Description = data.Description;
                    product.Category = data.Category;
                }
                response.Data = product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetProduct la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException(id, ex, _shareService.GetUsername(), _shareService.GetValue(), "GetProduct");
                return response;
            }

            await _logRepository.LogSuccess(id, response, _shareService.GetUsername(), _shareService.GetValue(), "GetProduct");
            return response;
        }

        public async Task<BaseResponse<List<Product>>> GetProductsByName(string productName)
        {
            _logger.LogCritical($"GetProductByName request at {DateTime.Now}");

            BaseResponse<List<Product>> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            try
            {
                List<Product> products = await _productRepository.GetProductsByName(productName);
                response.Data = products;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetProductsByName la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException(productName, ex, _shareService.GetUsername(), _shareService.GetValue(), "");
                return response;
            }
            await _logRepository.LogSuccess(productName, response, _shareService.GetUsername(), _shareService.GetValue(), "");
            return response;
        }

        public async Task<BaseResponse<ProductPaginationResultDTO>> GetProducts(GetProductsRequest request)
        {
            _logger.LogCritical($"GetProducts request at {DateTime.Now}");

            BaseResponse<ProductPaginationResultDTO> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            try
            {
                ProductPaginationResultDTO paginationResult = await _productRepository.GetProducts(request.PageNumber, request.ItemsPerPage, request.SortingColumn, request.SortingType, request.FilterColumn, request.FilterValue);
                if (request.FilterColumn == "ISBN" && string.IsNullOrEmpty(paginationResult.ProductList[0].Description))
                {
                    var product = paginationResult.ProductList[0];
                    var prices = _scrapperService.GetPriceFromAmazon(product.ISBN);
                    ScrappedData data = await _scrapperService.GetCatAndDesc(product.ISBN);
                    if (prices.Count > 0)
                    {
                        product.Price = Convert.ToInt32(prices[0]);
                    }
                    else
                    {
                        product.Price = 10;
                    }
                    product.Description = data.Description;
                    product.Category = data.Category;
                    await _productRepository.UpdateProductDetails(product.ISBN, product.Price, product.Description, product.Category);
                    paginationResult.ProductList[0] = product;
                }
                response.Data = paginationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetProducts la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException(request, ex, _shareService.GetUsername(), _shareService.GetValue(), "GetProducts");
                return response;
            }

            await _logRepository.LogSuccess(request, response, _shareService.GetUsername(), _shareService.GetValue(), "GetProducts");
            return response;
        }

        public async Task<BaseResponse<string>> AddOrRemoveFavorites(AddToFavoriteRequest request)
        {
            _logger.LogCritical($"AddOrRemoveFavorites request at {DateTime.Now}");

            BaseResponse<string> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            int userId = int.Parse(_shareService.GetUserId());

            try
            {
                await _productRepository.AddOrRemoveFavorites(request, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare AddOrRemoveFavorites la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException(request, ex, _shareService.GetUsername(), _shareService.GetValue(), "FavoriteHandler");
                return response;
            }
            await _logRepository.LogSuccess(request, response, _shareService.GetUsername(), _shareService.GetValue(), "FavoriteHandler");
            return response;
        }

        public async Task<BaseResponse<List<string>>> GetFavorites()
        {
            _logger.LogCritical($"GetFavorites request at {DateTime.Now}");

            BaseResponse<List<string>> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            string username = _shareService.GetUsername();

            if (String.IsNullOrEmpty(username))
            {
                response.IsSuccess = false;
                response.Message = "Token does no contain username information";
                return response;
            }

            try
            {
                var isbnList = await _productRepository.GetFavoriteProducts(username);
                response.Data = isbnList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetFavorites la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException("", ex, _shareService.GetUsername(), _shareService.GetValue(), "GetFavorites");
                return response;
            }
            await _logRepository.LogSuccess("", response, _shareService.GetUsername(), _shareService.GetValue(), "GetFavorites");
            return response;
        }

        public async Task<BaseResponse<List<ProductDTO>>> GetFavoriteProducts()
        {
            _logger.LogCritical($"GetFavoriteProduct request at {DateTime.Now}");

            BaseResponse<List<ProductDTO>> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            string username = _shareService.GetUsername();

            if (String.IsNullOrEmpty(username))
            {
                response.IsSuccess = false;
                response.Message = "Token does no contain username information";
                return response;
            }

            try
            {
                var isbnList = await _productRepository.GetFavoriteProducts(username);
                var productInfo = await _productRepository.GetProductsByIsbnList(isbnList);
                response.Data = productInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetFavoriteProduct la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException("", ex, _shareService.GetUsername(), _shareService.GetValue(), "GetFavoriteProductsInformation");
                return response;
            }
            await _logRepository.LogSuccess("", response, _shareService.GetUsername(), _shareService.GetValue(), "GetFavoriteProductsInformation");
            return response;
        }

        public async Task<BaseResponse<string>> RateProduct(RateProductRequest request)
        {
            _logger.LogCritical($"RateProduct request at {DateTime.Now}");

            BaseResponse<string> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            string userId = _shareService.GetUserId();

            if (String.IsNullOrEmpty(userId))
            {
                response.IsSuccess = false;
                response.Message = "Token does no contain userId information";
                return response;
            }

            try
            {
                string res = await _productRepository.InsertRating(request, userId);
                response.Data = res;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare RateProduct la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException(request, ex, _shareService.GetUsername(), _shareService.GetValue(), "RateProduct");
                return response;
            }
            await _logRepository.LogSuccess(request, response, _shareService.GetUsername(), _shareService.GetValue(), "RateProduct");
            return response;
        }
    }
}
