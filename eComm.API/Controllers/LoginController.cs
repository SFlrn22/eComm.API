﻿using eComm.APPLICATION.Contracts;
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
                return BadRequest(response.Message);

            var returnedUser = response.User.ToUserDTO();

            return Ok(new { User = returnedUser, AuthToken = response.Token });

        }
    }
}
