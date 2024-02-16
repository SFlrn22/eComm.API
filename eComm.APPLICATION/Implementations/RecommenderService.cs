using eComm.APPLICATION.Contracts;
using eComm.INFRASTRUCTURE.Contracts;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class RecommenderService : IRecommenderService
    {
        private readonly IExternalDepRepository _externalRepository;
        private readonly ILogger<LoginService> _logger;
        private readonly IShareService _shareService;
        public RecommenderService(IExternalDepRepository externalRepository, ILogger<LoginService> logger, IShareService shareService)
        {
            _externalRepository = externalRepository;
            _logger = logger;
            _shareService = shareService;
        }

        public async Task<List<string>> GetRecommendedItemsForId(string id)
        {
            _logger.LogInformation($"GetTopTen request la data {DateTime.Now}", _shareService.GetUsername(), _shareService.GetValue());
            try
            {
                List<string> topTen = await _externalRepository.GetRecommendedItemsForId(id);
                return topTen;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                throw;
            }
        }

        public async Task<List<string>> GetTopTen()
        {
            _logger.LogInformation($"GetTopTen request la data {DateTime.Now}", _shareService.GetUsername(), _shareService.GetValue());
            try
            {
                List<string> topTen = await _externalRepository.GetTopTen();
                return topTen;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString(), _shareService.GetUsername(), _shareService.GetValue());
                throw;
            }
        }
    }
}
