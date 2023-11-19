using eComm.APPLICATION.Contracts;
using eComm.INFRASTRUCTURE.Contracts;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class RecommenderService : IRecommenderService
    {
        private readonly IExternalDepRepository _externalRepository;
        private readonly ILogger<LoginService> _logger;
        public RecommenderService(IExternalDepRepository externalRepository, ILogger<LoginService> logger)
        {
            _externalRepository = externalRepository;
            _logger = logger;
        }
        public async Task<List<string>> GetTopTen()
        {
            _logger.LogInformation($"GetTopTen request la data {DateTime.Now}");
            try
            {
                List<string> topTen = await _externalRepository.GetTopTen();
                List<string> formattedResuult = new List<string>();
                foreach (string item in topTen)
                {
                    formattedResuult.Add(item.Substring(1, item.Length - 4).Trim());
                }
                return formattedResuult;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Eroare GetTopTen request la data {DateTime.Now}", ex.Message.ToString());
                throw;
            }
        }
    }
}
