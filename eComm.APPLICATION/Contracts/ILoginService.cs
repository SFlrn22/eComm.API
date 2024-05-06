using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;

namespace eComm.APPLICATION.Contracts
{
    public interface ILoginService
    {
        Task<BaseResponse<AuthResponse>> Authenticate(UserLoginRequest request);
        Task<BaseResponse<string>> Register(UserCreateRequest request);
        Task<BaseResponse<AuthResponse>> Refresh(TokenModel request);
    }
}
