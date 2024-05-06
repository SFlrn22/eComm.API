using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;

namespace eComm.APPLICATION.Contracts
{
    public interface IRecommenderService
    {
        Task<List<ProductDTO>> GetTopTen();
        Task<List<ProductDTO>> GetRecommendedItems(string id, string type);
        Task<List<AssociationRule>> GetAssociationRules(string title);
    }
}
