using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICarRepository _carRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository reviewRepository,ICarRepository carRepository,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _carRepository = carRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            try
            {
                return await _reviewRepository.GetAllReviewsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching all reviews.");

                throw;
            }
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            try
            {
                var review =await _reviewRepository.GetReviewByIdAsync(reviewId);

                if (review == null)
                    throw new KeyNotFoundException(
                        $"Review with ID {reviewId} not found.");

                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching review {ReviewId}",
                    reviewId);

                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId)
        {
            try
            {
                return await _reviewRepository.GetReviewsByCarIdAsync(carId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching reviews for Car {CarId}",
                    carId);

                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(int userId)
        {
            try
            {
                return await _reviewRepository.GetReviewsByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching reviews for User {UserId}",
                    userId);

                throw;
            }
        }

        public async Task<bool> AddReviewAsync(int userId,ReviewDto reviewDto)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(reviewDto.CarId);

                if (car == null)
                    throw new KeyNotFoundException(
                        "Car not found.");

                if (reviewDto.Rating < 1 ||
                    reviewDto.Rating > 5)
                    throw new ArgumentException(
                        "Rating must be between 1 and 5.");

                var review = new Review
                    {
                        UserId = userId,
                        CarId = reviewDto.CarId,
                        Rating = reviewDto.Rating,
                        Comment = reviewDto.Comment,
                        CreatedAt = DateTime.UtcNow
                    };

                await _reviewRepository.AddReviewAsync(review);

                var result = await _reviewRepository.SaveChangesAsync();

                _logger.LogInformation("Review added successfully by User {UserId} for Car {CarId}",
                  userId,reviewDto.CarId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,"Error occurred while adding review.");

                throw;
            }
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByIdAsync(reviewId);

                if (review == null)
                    throw new KeyNotFoundException(
                        $"Review with ID {reviewId} not found.");

                await _reviewRepository.DeleteReviewAsync(review);

                var result = await _reviewRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Review deleted successfully. ReviewId: {ReviewId}",
                    reviewId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while deleting review {ReviewId}",reviewId);

                throw;
            }
        }

        public async Task<PagedResponse<Review>> GetPagedReviewsAsync(PaginationParams paginationParams)
        {
            try
            {
                return await _reviewRepository.GetPagedReviewsAsync(paginationParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching paged reviews.");

                throw;
            }
        }
    }
}