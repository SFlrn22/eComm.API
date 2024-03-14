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
                    string username = securityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value!;
                    LogContext.PushProperty("Username", _shareService.GetUsername());
                    LogContext.PushProperty("SessionIdentifier", _shareService.GetValue());
                }
            }
            await _next(context);
        }
    }
}
