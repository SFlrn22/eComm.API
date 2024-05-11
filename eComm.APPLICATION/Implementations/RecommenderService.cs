using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
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

        public async Task<List<AssociationRule>> GetAssociationRules(string title)
        {
            _logger.LogInformation($"GetAssociationRules request la data {DateTime.Now}");
            try
            {
                List<AssociationRule> rules = new List<AssociationRule>();

                string isbn = await _productRepository.GetIsbnByTitle(title);

                List<string> isbnList = await _externalRepository.GetRecommendedItemsForId(isbn, "content");

                var options = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 20
                };

                await Parallel.ForEachAsync(isbnList, options, async (recommendation, ct) =>
                {
                    List<string> secondRecommendations = await _externalRepository.GetRecommendedItemsForId(recommendation, "content");

                    var intersection = isbnList.Intersect(secondRecommendations).ToList();

                    if (intersection.Count != 0)
                    {
                        var recommendationInfo = await _productRepository.GetProductDetailsByIsbn(recommendation);

                        var productLists = await _productRepository.GetProductsByIsbnList(intersection);

                        foreach (var product in productLists)
                        {
                            if (product.ISBN != isbn && !rules.Select(t => t.AssociatedTitle).Contains(product.Title))
                            {
                                rules.Add(new AssociationRule
                                {
                                    Title = recommendationInfo.Title,
                                    ImageURL = recommendationInfo.ImageUrlL,
                                    AssociatedTitle = product.Title,
                                    AssociatedImageURL = product.ImageUrlL
                                });
                            }
                        }
                    }
                });

                return rules;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetAssociationRules request la data {DateTime.Now}", ex.Message.ToString());
                await _logRepository.LogException(title, ex, _shareService.GetUsername(), _shareService.GetValue(), $"GetAssociationRules");
                throw;
            }
        }

        public async Task<List<ProductDTO>> GetRecommendedItems(string id, string type)
        {
            _logger.LogInformation($"GetRecommendedItems request la data {DateTime.Now}");
            try
            {
                List<string> isbnList = await _externalRepository.GetRecommendedItemsForId(id, type);
                if (isbnList is null)
                {
                    return new List<ProductDTO>();
                }
                List<ProductDTO> products = await _productRepository.GetProductsByIsbnList(isbnList);
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
