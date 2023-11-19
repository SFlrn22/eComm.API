using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;

namespace eComm.APPLICATION.Contracts
{
    public interface ILoginService
    {
        Task<AuthResponse> Authenticate(UserLoginRequest request);
    }
}
