namespace eComm.APPLICATION.Contracts
{
    public interface IRecommenderService
    {
        Task<List<string>> GetTopTen();
    }
}
