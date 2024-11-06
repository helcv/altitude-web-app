using backend.DTOs;

namespace backend.Interfaces
{
    public interface IUserService
    {
        Task<CreateDto> CreateUserAsync(RegisterDto registerDto);
    }
}
