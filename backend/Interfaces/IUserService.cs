using backend.DTOs;
using CSharpFunctionalExtensions;

namespace backend.Interfaces
{
    public interface IUserService
    {
        Task<CreateDto> CreateUserAsync(RegisterDto registerDto);
        Task<Result<UserDto, MessageDto>> GetProfileAsync(string id);
        Task<Result<MessageDto, IEnumerable<string>>> UpdateUserDetailsAsync(string id, UpdateUserDto updateUserDto);
    }
}
