using eComm.DOMAIN.DTO;
using Microsoft.AspNetCore.Http;

namespace eComm.INFRASTRUCTURE.Contracts
{
    public interface IExternalDepRepository
    {
        Task<List<string>> GetTopTen();
        Task<List<string>> GetRecommendedItemsForId(string id, string type);
        Task<ProductDTO> GetProductFromVoiceRecord(IFormFile file);
    }
}
