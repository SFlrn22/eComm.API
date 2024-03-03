namespace eComm.APPLICATION.Contracts
{
    public interface IRecommenderService
    {
        Task<List<string>> GetTopTen();
        Task<List<string>> GetRecommendedItems(string id, string type);
    }
}
