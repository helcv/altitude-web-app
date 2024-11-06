using backend.Authentication;
using backend.DTOs;
using backend.Entities;
using backend.Interfaces;
using CSharpFunctionalExtensions;
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
        public async Task<Result<TokenDto, MessageDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Invalid email."});
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Invalid password." });
            }

            var tokenDto = new TokenDto
            {
                Id = user.Id,
                Token = _tokenHandler.CreateToken(user),
            };

            return Result.Success<TokenDto, MessageDto>(tokenDto);
        }
    }
}
