using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result.IsFailure) return Unauthorized(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("signin-google")]
        public async Task<IActionResult> SignInGoogleAsync(GoogleTokenDto googleTokenDto)
        {
            var result = await _authService.GoogleSignIn(googleTokenDto);
            if (result.Token == null) return Unauthorized(result);

            return Ok(result);
        }

        [HttpGet("emailconfirmation")]
        public async Task<IActionResult> EmailConfirmationAsync([FromQuery] ConfirmEmailDto confirmEmailDto)
        {
            var result = await _authService.EmailConfirmationAsync(confirmEmailDto);
            if (!result) return BadRequest("Invalid Email Confirmation Request");

            return Ok();
        }
    }
}
