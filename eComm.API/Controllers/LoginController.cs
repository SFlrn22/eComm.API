using eComm.APPLICATION.Contracts;
using eComm.APPLICATION.ExtensionMethods;
using eComm.DOMAIN.Requests;
using eComm.DOMAIN.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userCredentials)
        {
            if (userCredentials == null)
                return BadRequest();

            AuthResponse response = await _loginService.Authenticate(userCredentials);

            if (!response.Message.IsNullOrEmpty())
                return Ok(new { message = response.Message });

            var returnedUser = response.User.ToUserDTO();

            return Ok(new { User = returnedUser, AuthToken = response.Token });

        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest request)
        {
            if (request == null)
                return BadRequest();

            string resp = await _loginService.Register(request);

            if (resp != "Success")
            {
                return BadRequest(resp);
            }

            return Ok("User creat cu succes");
        }
    }
}
