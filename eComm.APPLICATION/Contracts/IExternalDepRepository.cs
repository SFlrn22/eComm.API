namespace eComm.INFRASTRUCTURE.Contracts
{
    public interface IExternalDepRepository
    {
        Task<List<string>> GetTopTen();
        Task<List<string>> GetRecommendedItemsForId(string Id);
    }
}
