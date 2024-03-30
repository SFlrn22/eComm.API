namespace eComm.APPLICATION.Contracts
{
    public interface IScrapperService
    {
        List<double> GetPriceFromAmazon(string isbn);
    }
}
