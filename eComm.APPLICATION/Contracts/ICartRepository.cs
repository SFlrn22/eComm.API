using eComm.DOMAIN.DTO;

namespace eComm.APPLICATION.Contracts
{
    public interface ICartRepository
    {
        Task<string> AddToCart(int userId, int bookId, int count);
        Task RemoveFromCart(int userId, int bookId);
        Task<string> RenewCart(string sessionId);
        Task AddCartSession(int userId, string sessionId);
        Task<string> GetActiveSession(int userId);
        Task CompleteSession(string sessionId);
        Task<ActiveCartDTO> GetUserActiveCart(int userId);
    }
}
