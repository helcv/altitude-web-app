using backend.DTOs;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenDto, ErrorMessageDto>> LoginAsync(LoginDto loginDto);
    }
}
