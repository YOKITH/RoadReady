using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();

        Task<Review?> GetReviewByIdAsync(int reviewId);

        Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId);

        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);

        Task AddReviewAsync(Review review);

        Task UpdateReviewAsync(Review review);

        Task DeleteReviewAsync(Review review);

        Task<bool> SaveChangesAsync();
        Task<PagedResponse<Review>> GetPagedReviewsAsync(PaginationParams paginationParams);
    }
}