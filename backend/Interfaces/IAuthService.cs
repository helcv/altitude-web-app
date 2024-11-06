using backend.DTOs;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenDto, MessageDto>> LoginAsync(LoginDto loginDto);
    }
}
