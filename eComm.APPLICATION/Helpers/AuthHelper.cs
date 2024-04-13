using eComm.DOMAIN.Models;
using eComm.DOMAIN.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eComm.APPLICATION.Helpers
{
    public class AuthHelper
    {
        private readonly AppSettings _appSettings;
        public AuthHelper(IOptions<AppSettings> appsettings)
        {
            _appSettings = appsettings.Value;
        }
        public string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            string sessionidentifier = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Firstname),
                new Claim(ClaimTypes.Surname, user.Lastname),
                new Claim(type: "Identifier", value: sessionidentifier),
                new Claim(type: "UserId", value: user.ID.ToString())
            };

            var token = new JwtSecurityToken(_appSettings.Jwt.Issuer,
                _appSettings.Jwt.Audience,
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
