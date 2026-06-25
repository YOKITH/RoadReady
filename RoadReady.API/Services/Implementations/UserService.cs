using AutoMapper;
using BCrypt.Net;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Mappings;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository,ILogger<UserService> logger,IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching all users.");

                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user =await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                    throw new KeyNotFoundException(
                        $"User with ID {userId} not found.");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching user {UserId}",
                    userId);

                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user =await _userRepository.GetUserByEmailAsync(email);

                if (user == null)
                    throw new KeyNotFoundException(
                        $"User with email {email} not found.");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,"Error occurred while fetching user by email {Email}",
                    email);

                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                return await _userRepository.EmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,"Error occurred while checking email existence.");

                throw;
            }
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);

                if (existingUser != null)
                {
                    throw new ArgumentException("Email already exists.");
                }

                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                user.Role = "Customer";
                user.IsActive = true;

                user.CreatedAt = DateTime.UtcNow;

                await _userRepository.AddUserAsync(user);

                var result = await _userRepository.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Email}",user.Email);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError (ex,"Error occurred while registering user.");

                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId,User updatedUser)
        {
            try
            {
                var existingUser =await _userRepository.GetUserByIdAsync(userId);

                if (existingUser == null)
                    throw new KeyNotFoundException(
                        $"User with ID {userId} not found.");

                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;

                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateUserAsync(existingUser);

                var result = await _userRepository.SaveChangesAsync();

                _logger.LogInformation("User updated successfully: {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while updating user {UserId}",
                    userId);

                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user =await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                    throw new KeyNotFoundException(
                        $"User with ID {userId} not found.");

                await _userRepository.DeleteUserAsync(user);

                var result = await _userRepository.SaveChangesAsync();

                _logger.LogInformation("User deleted successfully: {UserId}",userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,"Error occurred while deleting user {UserId}",
                    userId);

                throw;
            }
        }

        public async Task<PagedResponse<User>> GetPagedUsersAsync(PaginationParams paginationParams)
        {
            try
            {
                return await _userRepository.GetPagedUsersAsync(paginationParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching paged users.");

                throw;
            }
        }
    }
}