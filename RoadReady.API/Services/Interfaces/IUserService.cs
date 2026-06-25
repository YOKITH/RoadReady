using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int userId);

        Task<User?> GetUserByEmailAsync(string email);

        Task<bool> RegisterUserAsync(RegisterDto registerDto);

        Task<bool> UpdateUserAsync(int userId, User updatedUser);

        Task<bool> DeleteUserAsync(int userId);

        Task<bool> EmailExistsAsync(string email);
        Task<PagedResponse<User>> GetPagedUsersAsync(PaginationParams paginationParams);
}
}