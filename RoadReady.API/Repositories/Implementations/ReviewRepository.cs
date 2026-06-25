using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;

namespace RoadReady.API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews.Include(r => r.User).Include(r => r.Car).ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews.Include(r => r.User).Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId)
        {
            return await _context.Reviews
                .Where(r => r.CarId == carId).Include(r => r.User).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
        {
            return await _context.Reviews.Where(r => r.UserId == userId)
                .Include(r => r.Car).ToListAsync();
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            return Task.CompletedTask;
        }

        public Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResponse<Review>> GetPagedReviewsAsync(PaginationParams paginationParams)
        {
            var query = _context.Reviews.Include(r => r.User)
                .Include(r => r.Car).AsQueryable();

            var totalRecords = await query.CountAsync();

            var reviews = await query
                .Skip((paginationParams.PageNumber - 1)* paginationParams.PageSize)
                .Take(paginationParams.PageSize).ToListAsync();

            return new PagedResponse<Review>
            {
                Data = reviews,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationParams.PageSize)
            };
        }
    }
}