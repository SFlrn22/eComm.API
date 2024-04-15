using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Responses;

namespace eComm.APPLICATION.Contracts
{
    public interface ICartManagementService
    {
        Task<BaseResponse<string>> AddToCart(int bookId, int count);
        Task<BaseResponse<string>> RemoveFromCart(int bookId);
        Task<BaseResponse<ActiveCartDTO>> GetActiveCart();
    }
}
