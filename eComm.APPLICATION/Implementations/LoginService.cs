using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.Helpers;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.PERSISTENCE.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eComm.APPLICATION.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthHelper _authHelper;
        private readonly IConfiguration _config;
        private readonly ILogger<LoginService> _logger;
        public LoginService(IUserRepository userRepository, IConfiguration config, ILogger<LoginService> logger)
        {
            _userRepository = userRepository;
            _config = config;
            _authHelper = new AuthHelper(_config);
            _logger = logger;
        }

        public async Task<AuthResponse> Authenticate(UserLoginRequest request)
        {
            _logger.LogTrace($"Auth request at {DateTime.Now}");
            AuthResponse resp = new AuthResponse();
            User returnedUser = new User();
            try
            {
                returnedUser = await _userRepository.GetUser(request.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare auth la {DateTime.Now}", ex.Message.ToString());
                resp.Message = "Exceptie, verifica logs";
                return resp;
            }
            if (returnedUser == null)
            {
                resp.Message = "Userul nu a fost gasit";
                return resp;
            }
            if (returnedUser.Password != request.Password)
            {
                resp.Message = "Username sau parola gresita";
                return resp;
            }
            string token = _authHelper.Generate(resp.User!);
            resp.User = returnedUser;
            resp.Token = token;
            return resp;

        }
    }
}
