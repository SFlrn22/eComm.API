using eComm.DOMAIN.Models;

namespace eComm.DOMAIN.Responses
{
    public class AuthResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public User User { get; set; } = new User();
    }
}
