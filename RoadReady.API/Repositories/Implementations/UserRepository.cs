using System;
using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.FirstName).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                                 .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResponse<User>> GetPagedUsersAsync(PaginationParams paginationParams)
        {
            var query = _context.Users.AsQueryable();

            var totalRecords = await query.CountAsync();

            var users = await query
                .Skip((paginationParams.PageNumber - 1)
                    * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResponse<User>
            {
                Data = users,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalRecords = totalRecords,
                TotalPages =
                    (int)Math.Ceiling(totalRecords / (double)paginationParams.PageSize)
            };
        }
    }
}