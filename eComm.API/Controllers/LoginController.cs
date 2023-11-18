using eComm.APPLICATION.Helpers;
using eComm.APPLICATION.Implementations;
using eComm.DOMAIN.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eComm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly LoginService _loginService;
        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userCredentials)
        {
            if (userCredentials == null)
                return BadRequest(ModelState);

            var userExistent = _loginService.Authenticate(userCredentials);

            if (userExistent == null)
                return NotFound();

            var token = AuthHelper.Generate(userExistent);
            //var returnedUser = _mapper.Map<UserDTO>(userExistent);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new { User = returnedUser, Token = token });

        }
    }
}
