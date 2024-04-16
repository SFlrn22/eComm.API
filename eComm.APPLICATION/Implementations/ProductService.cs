﻿using eComm.APPLICATION.Contracts;
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
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IShareService shareService, IScrapperService scrapperService)
        {
            _productRepository = productRepository;
            _logger = logger;
            _shareService = shareService;
            _scrapperService = scrapperService;
        }

        public async Task<BaseResponse<Product>> GetProduct(int id)
        {
            //LogContext.PushProperty("Username", _shareService.GetUsername());
            //LogContext.PushProperty("SessionIdentifier", _shareService.GetValue());
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
                return response;
            }

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
                return response;
            }

            return response;
        }

        public async Task<BaseResponse<ProductPaginationResultDTO>> GetProducts(int pageNumber, int itemsPerPage, string? sortingColumn, string? sortingType, string? filterColumn, string? filterValue)
        {
            //LogContext.PushProperty("Username", _shareService.GetUsername());
            //LogContext.PushProperty("SessionIdentifier", _shareService.GetValue());
            _logger.LogCritical($"GetProducts request at {DateTime.Now}");

            BaseResponse<ProductPaginationResultDTO> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            try
            {
                ProductPaginationResultDTO paginationResult = await _productRepository.GetProducts(pageNumber, itemsPerPage, sortingColumn, sortingType, filterColumn, filterValue);
                response.Data = paginationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetProducts la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

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
                return response;
            }

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
                return response;
            }

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
                return response;
            }

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
                return response;
            }

            return response;
        }
    }
}
