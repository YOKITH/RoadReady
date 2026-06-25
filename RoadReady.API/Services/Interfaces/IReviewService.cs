using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();

        Task<Review?> GetReviewByIdAsync(int reviewId);

        Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId);

        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId);

        Task<bool> AddReviewAsync(int userId, ReviewDto reviewDto);

        Task<bool> DeleteReviewAsync(int reviewId);
        Task<PagedResponse<Review>> GetPagedReviewsAsync(PaginationParams paginationParams);
    }
}