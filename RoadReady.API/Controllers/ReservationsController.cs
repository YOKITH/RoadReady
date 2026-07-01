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
    public class ReservationsController : ControllerBase
    {
        private readonly RoadReadyDbContext _context;

        public ReservationsController(RoadReadyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var car = await _context.Cars.FindAsync(dto.CarId);
            if (car == null)
            {
                return NotFound(new { message = "Car not found." });
            }

            if (!car.IsAvailable)
            {
                return BadRequest(new { message = "Car is currently not available for rent." });
            }

            // Calculate total price
            var totalDays = (dto.EndDate.Date - dto.StartDate.Date).Days;
            if (totalDays <= 0) totalDays = 1; // Minimum 1 day rent

            var totalPrice = car.PricePerDay * totalDays;

            var reservation = new Reservation
            {
                CarId = dto.CarId,
                UserId = userId.Value,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = totalPrice,
                Status = "Pending"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Reservation created successfully.",
                reservationId = reservation.Id,
                totalPrice = reservation.TotalPrice
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reservations);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reservations.Select(r => new
            {
                r.Id,
                r.StartDate,
                r.EndDate,
                r.TotalPrice,
                r.Status,
                r.CreatedAt,
                Car = new { r.Car!.Id, r.Car.Make, r.Car.Model, r.Car.ImageUrl },
                User = new { r.User!.Id, r.User.FullName, r.User.Email }
            }));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found." });
            }

            var userId = GetCurrentUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Check permission: Admin can change to any status, User can only cancel their own Pending/Confirmed booking
            if (userRole != "Admin")
            {
                if (reservation.UserId != userId)
                {
                    return Forbid();
                }

                if (dto.Status != "Cancelled")
                {
                    return BadRequest(new { message = "Users are only allowed to cancel reservations." });
                }

                if (reservation.Status == "Completed" || reservation.Status == "Cancelled")
                {
                    return BadRequest(new { message = "Cannot cancel a completed or already cancelled reservation." });
                }
            }

            reservation.Status = dto.Status;

            // If reservation status is Confirmed or Completed, make car unavailable. If Cancelled, make it available again.
            var car = await _context.Cars.FindAsync(reservation.CarId);
            if (car != null)
            {
                if (dto.Status == "Cancelled" || dto.Status == "Completed")
                {
                    car.IsAvailable = true;
                }
                else if (dto.Status == "Confirmed")
                {
                    car.IsAvailable = false;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Reservation status updated to {dto.Status}.", status = dto.Status });
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

    // DTOs
    public class CreateReservationDto
    {
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty; // Confirmed, Completed, Cancelled
    }
}
