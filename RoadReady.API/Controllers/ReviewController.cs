using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadReady.API.DTOs;
using RoadReady.API.Pagination;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

  
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);

            if (review == null)
                return NotFound("Review not found");

            return Ok(review);
        }

        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetReviewsByCar(int carId)
        {
            var reviews = await _reviewService.GetReviewsByCarIdAsync(carId);

            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewsByUser(int userId)
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);

            return Ok(reviews);
        }

       
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _reviewService.AddReviewAsync(userId,reviewDto);

            if (!result)
                return BadRequest("Unable to add review");

            return Ok("Review added successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);

            if (!result)
                return NotFound("Review not found");

            return Ok("Review deleted successfully");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedReviews([FromQuery] PaginationParams paginationParams)
        {
            var reviews = await _reviewService.GetPagedReviewsAsync(paginationParams);

            return Ok(reviews);
        }
    }
}