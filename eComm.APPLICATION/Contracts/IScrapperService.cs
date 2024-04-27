using eComm.DOMAIN.Models;
using Microsoft.AspNetCore.Http;

namespace eComm.APPLICATION.Contracts
{
    public interface IScrapperService
    {
        List<double> GetPriceFromAmazon(string isbn);
        Task<ScrappedData> GetCatAndDesc(string isbn);
        Task<ImageSource> GetImageSource(IFormFile file);
    }
}
