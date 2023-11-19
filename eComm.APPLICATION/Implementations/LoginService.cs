using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Helpers;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Configuration;

namespace eComm.APPLICATION.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthHelper _authHelper;
        private readonly IConfiguration _config;
        public LoginService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
            _authHelper = new AuthHelper(_config);
        }

        public async Task<AuthResponse> Authenticate(UserLoginRequest request)
        {
            AuthResponse resp = new AuthResponse();

            User returnedUser = await _userRepository.GetUser(request.Username);
            if (returnedUser == null)
            {
                resp.Message = "Userul nu a fost gasit";
                return resp;
            }
            if (returnedUser.Password != returnedUser.Password)
            {
                resp.Message = "Username sau parola gresita";
                return resp;
            }
            string token = _authHelper.Generate(resp.User!);
            resp.Token = token;
            return resp;

        }
    }
}
