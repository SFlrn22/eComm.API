using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eComm.IntegrationTests.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _configuration;
        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string HandleAuth()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test"),
                new Claim(ClaimTypes.Email, "test"),
                new Claim(ClaimTypes.GivenName, "test"),
                new Claim(ClaimTypes.Surname, "test"),
                new Claim(type: "Identifier", value: Guid.NewGuid().ToString()),
                new Claim(type: "UserId", value: "id")
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:JwtConfiguration:Key").Value!));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _configuration.GetSection("AppSettings:JwtConfiguration:Issuer").Value!,
                    audience: _configuration.GetSection("AppSettings:JwtConfiguration:Audience").Value!,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signingCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;

        }
    }
}
