using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.ExtensionMethods;
using eComm.APPLICATION.Helpers;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Contracts;
using eComm.PERSISTENCE.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace eComm.APPLICATION.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthHelper _authHelper;
        private readonly ILogger<LoginService> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        public LoginService(IUserRepository userRepository, ILogger<LoginService> logger, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _appSettings = appSettings;
            _authHelper = new AuthHelper(_appSettings);
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
            if (returnedUser.Password != EncryptionHelper.Sha256Hash(request.Password))
            {
                response.IsSuccess = false;
                response.Message = "Username sau parola gresita";
                return response;
            }

            string token = _authHelper.Generate(returnedUser);
            string refreshToken = _authHelper.GenerateRefreshToken();
            string hashedToken = EncryptionHelper.Sha256Hash(refreshToken);
            await _userRepository.UpdateRefreshExpireDate(DateTime.Now.AddHours(7), request.Username, hashedToken);
            resp.User = returnedUser.ToUserDTO();
            resp.Token = token;
            resp.RefreshToken = refreshToken;
            response.Data = resp;

            return response;
        }

        public async Task<BaseResponse<AuthResponse>> Refresh(TokenModel request)
        {
            _logger.LogCritical($"RefreshToken request at {DateTime.Now}");

            BaseResponse<AuthResponse> response = new()
            {
                IsSuccess = true,
                Message = "Refresh successful"
            };

            var claims = _authHelper.GetPrincipalFromExpiredToken(request.Token);
            var username = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value!;


            AuthResponse authResponse = new AuthResponse();

            try
            {
                var returnedUser = await _userRepository.GetUser(username);

                var hashedToken = EncryptionHelper.Sha256Hash(request.RefreshToken);

                if (returnedUser is null || returnedUser.RefreshExpireDate < DateTime.Now || hashedToken != returnedUser.RefreshToken)
                {
                    response.IsSuccess = false;
                    response.Message = "Unauthorized";
                    return response;
                }

                var newToken = _authHelper.Generate(returnedUser);

                authResponse.Token = newToken;
                authResponse.RefreshToken = request.RefreshToken;

                response.Data = authResponse;

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Eroare Refresh la {DateTime.Now}", ex.Message.ToString());
                response.IsSuccess = false;
                response.Message = "Exception";
                return response;
            }
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
                request.Password = EncryptionHelper.Sha256Hash(request.Password);
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
