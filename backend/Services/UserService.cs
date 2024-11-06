using backend.DTOs;
using backend.Entities;
using backend.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository; 
            _userManager = userManager;
        }
        public async Task<CreateDto> CreateUserAsync(RegisterDto registerDto)
        {
            var messages = new List<string>();

            var userToRegister = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Name = registerDto.Name,
                LastName = registerDto.LastName,
                DateOfBirth = registerDto.DateOfBirth ?? DateOnly.MinValue
            };

            var result = await _userManager.CreateAsync(userToRegister, registerDto.Password);
            if (!result.Succeeded)
            {
               messages.AddRange(result.Errors.Select(error => error.Description));
               return new CreateDto { Id = null, Messages = messages };
            }

            messages.Add("User successfully created!");
            return new CreateDto { Id = userToRegister.Id, Messages = messages };
        }

        public async Task<Result<UserDto, ErrorMessageDto>> GetProfileAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.Failure<UserDto, ErrorMessageDto>(new ErrorMessageDto { Message = "User does not exist." });
            }

            var userToReturn = new UserDto
            {
                Email = user.Email,
                Name = user.Name,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };

            return Result.Success<UserDto, ErrorMessageDto>(userToReturn);
        }
    }
}
