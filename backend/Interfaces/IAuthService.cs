using backend.DTOs;
using backend.Entities;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenDto, MessageDto>> LoginAsync(LoginDto loginDto);
        Task<GoogleSignInDto> GoogleSignIn(GoogleTokenDto googleTokenDto);
        Task<bool> EmailConfirmationAsync(ConfirmEmailDto confirmEmailDto);
        Task EnableTwoFactorAsync(User user, bool isEnabled);
        Task<TokenDto> TwoFactorAsync(TwoFactorDto twoFactorDto);
    }
}
