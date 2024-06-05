using eComm.DOMAIN.Models;
using System.Security.Claims;

namespace eComm.APPLICATION.Contracts
{
    public interface IAuthHelper
    {
        string Generate(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
