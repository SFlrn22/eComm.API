using eComm.DOMAIN.DTO;

namespace eComm.APPLICATION.Contracts
{
    public interface ICartRepository
    {
        Task<string> AddToCart(int userId, int bookId);
        Task RemoveFromCart(int userId, int bookId);
        Task<string> RenewCart(int userId);
        Task AddCartSession(int userId, string sessionId);
        Task<string> GetActiveSession(int userId);
        Task<ActiveCartDTO> GetUserActiveCart(int userId);
    }
}
