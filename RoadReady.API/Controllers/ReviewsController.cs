using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly RoadReadyDbContext _context;

        public ReviewsController(RoadReadyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // Validate car exists
            var car = await _context.Cars.FindAsync(dto.CarId);
            if (car == null)
            {
                return NotFound(new { message = "Car not found." });
            }

            // Check if user has already reviewed this car (optional limit, let's allow multiple reviews or limit to one. Let's allow one review per car per user to avoid spam)
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.CarId == dto.CarId && r.UserId == userId.Value);

            if (existingReview != null)
            {
                // Update existing review instead of creating duplicate
                existingReview.Rating = dto.Rating;
                existingReview.Comment = dto.Comment;
                existingReview.CreatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Review updated successfully." });
            }

            var review = new Review
            {
                CarId = dto.CarId,
                UserId = userId.Value,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review submitted successfully." });
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int id))
            {
                return id;
            }
            return null;
        }
    }

    // DTO
    public class AddReviewDto
    {
        public int CarId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
