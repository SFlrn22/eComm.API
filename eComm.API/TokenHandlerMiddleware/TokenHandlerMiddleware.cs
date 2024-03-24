using eComm.APPLICATION.Contracts;
using Serilog.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eComm.API.TokenHandlerMiddleware
{
    public class TokenHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IShareService _shareService;
        public TokenHandlerMiddleware(RequestDelegate next, IShareService shareService)
        {
            _next = next;
            _shareService = shareService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? header = context.Request.Headers.Authorization.FirstOrDefault();
            if (header is not null && header.Split(" ")[0] == "Bearer")
            {
                string token = header.Split(" ")[1];
                if (token is not null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                    string identifier = securityToken.Claims.FirstOrDefault(c => c.Type == "Identifier")!.Value!;
                    string userId = securityToken.Claims.FirstOrDefault(c => c.Type == "UserId")!.Value!;
                    string username = securityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value!;
                    _shareService.SetValue(identifier);
                    _shareService.SetUsername(username);
                    _shareService.SetUserId(userId);
                    LogContext.PushProperty("Username", username);
                    LogContext.PushProperty("SessionIdentifier", identifier);
                }
            }
            await _next(context);
        }
    }
}
