using backend.DTOs;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IUserService
    {
        Task<CreateDto> CreateUserAsync(RegisterDto registerDto);
        Task<Result<UserDto, ErrorMessageDto>> GetProfileAsync(string id);
    }
}
