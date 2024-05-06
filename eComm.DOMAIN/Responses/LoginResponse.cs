using eComm.DOMAIN.DTO;

namespace eComm.DOMAIN.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new UserDTO();
    }
}
