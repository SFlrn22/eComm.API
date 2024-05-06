using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost("/api/Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userCredentials)
        {
            if (userCredentials == null)
                return BadRequest();

            BaseResponse<AuthResponse> response = await _loginService.Authenticate(userCredentials);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);

        }
        [AllowAnonymous]
        [HttpPost("/api/Register")]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest request)
        {
            if (request == null)
                return BadRequest();

            BaseResponse<string> response = await _loginService.Register(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("/api/Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel request)
        {
            if (request == null)
                return BadRequest();

            var response = await _loginService.Refresh(request);

            return Ok(response);
        }
    }
}
