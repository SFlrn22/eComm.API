namespace eComm.APPLICATION.Contracts
{
    public interface ICartRepository
    {
        Task<string> AddToCart(int userId, int bookId);
        Task RemoveFromCart(int userId, int bookId);
        Task<string> RenewCart(int userId);
    }
}
