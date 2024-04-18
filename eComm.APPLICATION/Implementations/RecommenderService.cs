using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.INFRASTRUCTURE.Contracts;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class RecommenderService : IRecommenderService
    {
        private readonly IExternalDepRepository _externalRepository;
        private readonly ILogger<LoginService> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ILogRepository _logRepository;
        private readonly IShareService _shareService;
        public RecommenderService(IExternalDepRepository externalRepository, ILogger<LoginService> logger, IProductRepository productRepository, ILogRepository logRepository, IShareService shareService)
        {
            _externalRepository = externalRepository;
            _logger = logger;
            _productRepository = productRepository;
            _logRepository = logRepository;
            _shareService = shareService;
        }

        public async Task<List<ProductDTO>> GetRecommendedItems(string id, string type)
        {
            _logger.LogInformation($"GetTopTen request la data {DateTime.Now}");
            try
            {
                List<string> titleList = await _externalRepository.GetRecommendedItemsForId(id, type);
                if (titleList is null)
                {
                    return new List<ProductDTO>();
                }
                List<ProductDTO> products = await _productRepository.GetProductsByIsbnList(titleList);
                await _logRepository.LogSuccess(new { id, type }, products, _shareService.GetUsername(), _shareService.GetValue(), $"{type}based");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString());
                await _logRepository.LogException(new { id, type }, ex, _shareService.GetUsername(), _shareService.GetValue(), $"{type}based");
                throw;
            }
        }

        public async Task<List<ProductDTO>> GetTopTen()
        {
            _logger.LogInformation($"GetTopTen request la data {DateTime.Now}");
            try
            {
                List<string> isbnList = await _externalRepository.GetTopTen();
                List<ProductDTO> products = await _productRepository.GetProductsByIsbnList(isbnList);
                await _logRepository.LogSuccess("", products, _shareService.GetUsername(), _shareService.GetValue(), "GetTopTen");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString());
                await _logRepository.LogException("", ex, _shareService.GetUsername(), _shareService.GetValue(), "GetTopTen");
                throw;
            }
        }
    }
}
