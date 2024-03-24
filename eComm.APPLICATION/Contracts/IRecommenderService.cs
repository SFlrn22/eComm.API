using eComm.DOMAIN.DTO;

namespace eComm.APPLICATION.Contracts
{
    public interface IRecommenderService
    {
        Task<List<ProductDTO>> GetTopTen();
        Task<List<ProductDTO>> GetRecommendedItems(string id, string type);
    }
}
