using eComm.DOMAIN.Models;

namespace eComm.APPLICATION.Contracts
{
    public interface IScrapperService
    {
        List<double> GetPriceFromAmazon(string isbn);
        Task<ScrappedData> GetCatAndDesc(string isbn);
    }
}
