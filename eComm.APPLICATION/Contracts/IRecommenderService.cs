using eComm.DOMAIN.DTO;

namespace eComm.APPLICATION.Contracts
{
    public interface IRecommenderService
    {
        Task<List<TopProductsDTO>> GetTopTen();
        Task<List<string>> GetRecommendedItems(string id, string type);
    }
}
