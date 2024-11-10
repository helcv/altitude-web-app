using AutoMapper;
using backend.Constants;
using backend.DTOs;
using backend.Entities;
using backend.Helpers;
using backend.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private const int minAge = 15;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository, UserManager<User> userManager, 
                IFileService fileService, IMapper mapper, IEmailService emailService)
        {
            _mapper = mapper;
            _userRepository = userRepository; 
            _userManager = userManager;
            _fileService = fileService;
            _emailService = emailService;
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

            var age = CalculateAge(registerDto.DateOfBirth.Value);
            if (age < minAge)
            {
                messages.Add($"You must be at least {minAge} years old.");
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

            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(userToRegister);
            var param = new Dictionary<string, string>
            {
                { "token", confirmToken },
                { "email", userToRegister.Email }
            };

            var callback = QueryHelpers.AddQueryString(registerDto.ClientUri, param);
            var message = new EmailMessageDto { Callback = callback, Email = userToRegister.Email, Subject = "Email confirmation token" };

            await _emailService.SendEmail(message);

            var addToRole = await _userManager.AddToRoleAsync(userToRegister, Roles.User);
            if (!addToRole.Succeeded)
            {
                messages.Add("Role does not exist.");
                return new CreateDto { Id = null, Messages = messages };
            }

            messages.Add("User successfully created, verify email address!");
            return new CreateDto { Id = userToRegister.Id, Messages = messages };
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (await _userManager.IsInRoleAsync(user, Roles.Admin))
                return false;

            if (!await _userRepository.DeleteUserAsync(id))
                return false;

            await _userRepository.SaveAllAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(string searchTerm, FilterParams filterParams = null)
        {
            var users = _userRepository.GetAllUsers();

            if (filterParams != null && filterParams.StartDate != DateOnly.MinValue && filterParams.EndDate != DateOnly.MinValue)
            {
                users = users.Where(u => u.DateOfBirth >= filterParams.StartDate &&
                    u.DateOfBirth <= filterParams.EndDate);
            }

            if (filterParams.IsVerified != null)
            {
                users = users.Where(u => u.EmailConfirmed == filterParams.IsVerified);
            }

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
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsTwoFactorEnabled = user.TwoFactorEnabled
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

            var age = CalculateAge(updateUserDto.DateOfBirth.Value);
            if (age < minAge)
            {
                errorMessages.Add($"You must be at least {minAge} years old.");
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

        private int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            int age = today.Year - dateOfBirth.Year;

            if (today < dateOfBirth.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
