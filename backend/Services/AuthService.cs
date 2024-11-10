using backend.Authentication;
using backend.DTOs;
using backend.Entities;
using backend.Interfaces;
using CSharpFunctionalExtensions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using backend.Constants;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IAuthTokenHandler _tokenHandler;
        private readonly string _googleCredentials;

        public AuthService(UserManager<User> userManager, IAuthTokenHandler tokenHandler, IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _googleCredentials = config["GoogleAuth:ClientId"];
        }

        public async Task<bool> EmailConfirmationAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user == null) return false;

            var confirmResult = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);
            if (!confirmResult.Succeeded) return false;

            return true;
        }

        public async Task<GoogleSignInDto> GoogleSignIn(GoogleTokenDto googleTokenDto)
        {
            var messages = new List<string>();
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokenDto.Token);

            if (payload.Audience.ToString() != _googleCredentials)
            {
                throw new InvalidJwtException("Invalid token");
            }

            var userExist = await _userManager.FindByEmailAsync(payload.Email);
            if (userExist != null && userExist.PasswordHash == null) 
            {
                messages.Add("User logged in.");
                return new GoogleSignInDto { Id = userExist.Id , Messages = messages, Token = await _tokenHandler.CreateToken(userExist) };
            }

            var userToRegister = new User
            {
                Name = payload.GivenName,
                LastName = payload.FamilyName,
                Email = payload.Email,
                UserName = payload.Email
            };

            var result = await _userManager.CreateAsync(userToRegister);
            if (!result.Succeeded)
            {
                messages.AddRange(result.Errors.Select(error => error.Description));
                return new GoogleSignInDto { Id = null, Messages = messages, Token = null };
            }

            var addToRole = await _userManager.AddToRoleAsync(userToRegister, Roles.User);
            if (!addToRole.Succeeded)
            {
                messages.Add("Role does not exist.");
                return new GoogleSignInDto { Id = null, Messages = messages, Token = null };
            }

            messages.Add("User successfully created!");
            return new GoogleSignInDto { Id = userToRegister.Id, Messages = messages, Token = await _tokenHandler.CreateToken(userToRegister)};
        }

        public async Task<Result<TokenDto, MessageDto>> LoginAsync(LoginDto loginDto)
        {
            var userDeleted = await _userRepository.GetAllUsers()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsDeleted);

            if (userDeleted != null)
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Your account has been deleted." });
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Invalid email." });
            }
            if (user.PasswordHash == null)
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Use Google SignIn." });
            }
            if (!await _userManager.IsEmailConfirmedAsync(user) && !await _userManager.IsInRoleAsync(user, Roles.Admin))
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Please verify email address" });
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return Result.Failure<TokenDto, MessageDto>(new MessageDto { Message = "Invalid password." });
            }

            var tokenDto = new TokenDto
            {
                Id = user.Id,
                Token = await _tokenHandler.CreateToken(user),
            };

            return Result.Success<TokenDto, MessageDto>(tokenDto);
        }
    }
}
