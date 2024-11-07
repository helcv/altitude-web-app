using AutoMapper;
using backend.Constants;
using backend.DTOs;
using backend.Entities;
using backend.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, UserManager<User> userManager, 
                IFileService fileService, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository; 
            _userManager = userManager;
            _fileService = fileService;
        }
        public async Task<CreateDto> CreateUserAsync(RegisterDto registerDto)
        {
            var messages = new List<string>();

            var userExist = await _userRepository.GetAllUsers()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (userExist != null)
            {
                messages.Add($"Email '{registerDto.Email}' is already taken.");
                return new CreateDto { Id = null, Messages = messages };
            }

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

            var addToRole = await _userManager.AddToRoleAsync(userToRegister, Roles.User);
            if (!addToRole.Succeeded)
            {
                messages.Add("Role does not exist.");
                return new CreateDto { Id = null, Messages = messages };
            }

            messages.Add("User successfully created!");
            return new CreateDto { Id = userToRegister.Id, Messages = messages };
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            if (!await _userRepository.DeleteUserAsync(id))
            {
                return false;
            }

            await _userRepository.SaveAllAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(string searchTerm)
        {
            var users = _userRepository.GetAllUsers();

            if (DateOnly.TryParse(searchTerm, out DateOnly dateOfBirth))
            {
                users = users.Where(u => u.DateOfBirth == dateOfBirth);
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.Email.ToLower().Contains(searchTerm.ToLower()));
            }

            var usersToReturn = await users.ToListAsync();
            return _mapper.Map<List<UserDto>>(usersToReturn);
        }

        public async Task<Result<UserDto, MessageDto>> GetProfileAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.Failure<UserDto, MessageDto>(new MessageDto { Message = "User does not exist." });
            }

            var userToReturn = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                ProfilePhotoUrl = user.ProfilePhotoUrl
            };

            return Result.Success<UserDto, MessageDto>(userToReturn);
        }

        public async Task<Result<MessageDto, IEnumerable<string>>> UpdateUserDetailsAsync(string id, UpdateUserDto updateUserDto)
        {
            var errorMessages = new List<string>();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                errorMessages.Add("User does not exist.");
                return Result.Failure<MessageDto, IEnumerable<string>>(errorMessages);
            }

            user.Name = updateUserDto.Name;
            user.LastName = updateUserDto.LastName;
            user.DateOfBirth = updateUserDto.DateOfBirth ?? DateOnly.MinValue;

            if (updateUserDto.ProfilePhoto != null)
            {
                var photoResult = await _fileService.UploadFileAsync(updateUserDto.ProfilePhoto);

                if (photoResult.IsFailure)
                {
                    errorMessages.Add(photoResult.Error.Message);
                }

                _fileService.DeleteFile(user.ProfilePhotoUrl);
                user.ProfilePhotoUrl = photoResult.Value;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                errorMessages.AddRange(result.Errors.Select(error => error.Description));
                return Result.Failure<MessageDto, IEnumerable<string>>(errorMessages);
            }

            return Result.Success<MessageDto, IEnumerable<string>>(new MessageDto { Message = "User successfully updated." });

        }

        public async Task<Result<MessageDto, IEnumerable<string>>> UpdateUserPasswordAsync(string id, UpdatePasswordDto updatePasswordDto)
        {
            var errorMessages = new List<string>();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                errorMessages.Add("User does not exist.");
                return Result.Failure<MessageDto, IEnumerable<string>>(errorMessages);
            }

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordDto.OldPassword, updatePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                errorMessages.AddRange(result.Errors.Select(error => error.Description));
                return Result.Failure<MessageDto, IEnumerable<string>>(errorMessages);
            }

            return Result.Success<MessageDto, IEnumerable<string>>(new MessageDto { Message = "Password successfully updated." });
        }
    }
}
