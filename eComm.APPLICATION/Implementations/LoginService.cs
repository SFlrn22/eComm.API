using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.ExtensionMethods;
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

        public async Task<BaseResponse<AuthResponse>> Authenticate(UserLoginRequest request)
        {
            _logger.LogCritical($"Auth request at {DateTime.Now}");
            BaseResponse<AuthResponse> response = new()
            {
                IsSuccess = true,
                Message = "Login successful"
            };
            AuthResponse resp = new AuthResponse();
            User returnedUser = new User();
            try
            {
                returnedUser = await _userRepository.GetUser(request.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare auth la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            if (returnedUser == null)
            {
                response.IsSuccess = false;
                response.Message = "Userul nu a fost gasit";
                return response;
            }
            if (returnedUser.Password != request.Password)
            {
                response.IsSuccess = false;
                response.Message = "Username sau parola gresita";
                return response;
            }

            string token = _authHelper.Generate(returnedUser);
            resp.User = returnedUser.ToUserDTO();
            resp.Token = token;
            response.Data = resp;

            return response;
        }

        public async Task<BaseResponse<string>> Register(UserCreateRequest request)
        {
            int resp = 0;

            _logger.LogCritical($"Register request at {DateTime.Now}");

            BaseResponse<string> response = new()
            {
                IsSuccess = true,
                Message = "Register successful"
            };

            User returnedUser = new User();

            try
            {
                returnedUser = await _userRepository.GetUser(request.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare auth la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
            if (returnedUser != null)
            {
                if (returnedUser.Email == request.Email)
                {
                    response.IsSuccess = false;
                    response.Message = "Exista deja un cont cu aceasta adresa de email";
                    return response;
                }
                else if (returnedUser.Username == request.UserName)
                {
                    response.IsSuccess = false;
                    response.Message = "Exista deja un cont cu acest username";
                    return response;
                }
            }

            try
            {
                resp = await _userRepository.CreateUser(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare auth la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }

            if (resp == 0)
            {
                response.IsSuccess = false;
                response.Message = "Userul nu a putut fi creat";
                return response;
            }

            return response;
        }
    }
}
