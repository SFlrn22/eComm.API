using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Responses;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class CartManagementService : ICartManagementService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IShareService _shareService;
        private readonly ILogRepository _logRepository;
        public CartManagementService(ICartRepository cartRepository, ILogger<ProductService> logger, IShareService shareService, ILogRepository logRepository)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _shareService = shareService;
            _logRepository = logRepository;
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
                await _logRepository.LogException(new { bookId, count }, ex, _shareService.GetUsername(), _shareService.GetValue(), "AddToCart");
                return response;
            }
            await _logRepository.LogSuccess(new { bookId, count }, response, _shareService.GetUsername(), _shareService.GetValue(), "AddToCart");
            return response;
        }

        public async Task<BaseResponse<ActiveCartDTO>> GetActiveCart()
        {
            _logger.LogCritical($"GetActiveCart request at {DateTime.Now}");

            BaseResponse<ActiveCartDTO> response = new()
            {
                IsSuccess = true,
                Message = "Success"
            };

            string userId = _shareService.GetUserId();

            try
            {
                var res = await _cartRepository.GetUserActiveCart(int.Parse(userId));
                response.Data = res;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare GetActiveCart la {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                response.IsSuccess = false;
                response.Message = ex.Message;
                await _logRepository.LogException("", ex, _shareService.GetUsername(), _shareService.GetValue(), "GetActiveCart");
                return response;
            }
            await _logRepository.LogSuccess("", response, _shareService.GetUsername(), _shareService.GetValue(), "GetActiveCart");
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
                await _logRepository.LogException(bookId, ex, _shareService.GetUsername(), _shareService.GetValue(), "RemoveFromCart");
                return response;
            }
            await _logRepository.LogSuccess(bookId, response, _shareService.GetUsername(), _shareService.GetValue(), "RemoveFromCart");
            return response;
        }
    }
}
