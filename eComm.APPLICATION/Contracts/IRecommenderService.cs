using eComm.DOMAIN.DTO;

namespace eComm.APPLICATION.Contracts
{
    public interface IRecommenderService
    {
        Task<List<TopProductsDTO>> GetTopTen();
        Task<List<TopProductsDTO>> GetRecommendedItems(string id, string type);
    }
}
