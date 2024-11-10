using backend.DTOs;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenDto, MessageDto>> LoginAsync(LoginDto loginDto);
        Task<GoogleSignInDto> GoogleSignIn(GoogleTokenDto googleTokenDto);
        Task<bool> EmailConfirmationAsync(ConfirmEmailDto confirmEmailDto);
    }
}
