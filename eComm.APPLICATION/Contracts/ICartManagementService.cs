using eComm.DOMAIN.Responses;

namespace eComm.APPLICATION.Contracts
{
    public interface ICartManagementService
    {
        Task<BaseResponse<string>> AddToCart(int bookId);
        Task<BaseResponse<string>> RemoveFromCart(int bookId);
        Task<BaseResponse<string>> RenewCart();
    }
}
