using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int userId);

        Task<User?> GetUserByEmailAsync(string email);

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> SaveChangesAsync();
        Task<PagedResponse<User>> GetPagedUsersAsync(PaginationParams paginationParams);
}
}