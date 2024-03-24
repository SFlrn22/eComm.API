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
        public RecommenderService(IExternalDepRepository externalRepository, ILogger<LoginService> logger, IProductRepository productRepository)
        {
            _externalRepository = externalRepository;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<List<ProductDTO>> GetRecommendedItems(string id, string type)
        {
            _logger.LogInformation($"GetTopTen request la data {DateTime.Now}");
            try
            {
                List<string> titleList = await _externalRepository.GetRecommendedItemsForId(id, type);
                List<ProductDTO> products = await _productRepository.GetProductsByTitleList(titleList);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString());
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
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString());
                throw;
            }
        }
    }
}
