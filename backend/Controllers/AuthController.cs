using backend.Constants;
using backend.DTOs;
using backend.Entities;
using backend.Extensions;
using backend.Interfaces;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        public AuthController(IAuthService authService, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _authService = authService;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
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

        [Authorize]
        [HttpPut("twofactor")]
        public async Task<IActionResult> EnableTwoFactorAsync(EnableTwoFactorDto setTwoFactorDto)
        {
            var currentUserId = (_contextAccessor.HttpContext.User).GetUserId();

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (await _userManager.IsInRoleAsync(user, Roles.Admin) || user.IsAuthWithGoogle)
                return BadRequest("Cannot set two factor authentication for this user!");

            await _authService.EnableTwoFactorAsync(user, setTwoFactorDto.IsEnabled.Value);

            return Ok();
        }

        [HttpPost("twofactor")]
        public async Task<IActionResult> TwoFactorAsync(TwoFactorDto twoFactorDto)
        {
            var result = await _authService.TwoFactorAsync(twoFactorDto);
            if (result.Token == null) return BadRequest("Two factor auth failed.");

            return Ok(result);
        }
    }
}
