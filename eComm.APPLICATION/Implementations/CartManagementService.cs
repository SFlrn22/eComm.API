using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Responses;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class CartManagementService : ICartManagementService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IShareService _shareService;
        public CartManagementService(ICartRepository cartRepository, ILogger<ProductService> logger, IShareService shareService)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _shareService = shareService;
        }

        public async Task<BaseResponse<string>> AddToCart(int bookId, int count)
        {
            _logger.LogCritical($"AddToCart request at {DateTime.Now}");

            BaseResponse<string> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            string userId = _shareService.GetUserId();

            try
            {
                string res = await _cartRepository.AddToCart(int.Parse(userId), bookId, count);
                response.Data = res;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare AddToCart la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }

        public async Task<BaseResponse<string>> RemoveFromCart(int bookId)
        {
            _logger.LogCritical($"RemoveFromCart request at {DateTime.Now}");

            BaseResponse<string> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            string userId = _shareService.GetUserId();

            try
            {
                await _cartRepository.RemoveFromCart(int.Parse(userId), bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare RemoveFromCart la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

            return response;
        }
    }
}
