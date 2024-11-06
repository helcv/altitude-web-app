using backend.Authentication;
using backend.DTOs;
using backend.Entities;
using backend.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthTokenHandler _tokenHandler;

        public AuthService(UserManager<User> userManager, IAuthTokenHandler tokenHandler)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
        }
        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new TokenDto { Id = null, Token = null, Message = "Invalid email address." };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return new TokenDto { Id = null, Token = null, Message = "Invalid password." };
            }

            return new TokenDto
            {
                Id = user.Id,
                Token = _tokenHandler.CreateToken(user),
                Message = "User successfully logged in."
            };
        }
    }
}
