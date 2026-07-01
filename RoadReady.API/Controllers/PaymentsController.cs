using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using System;
using System.Threading.Tasks;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly RoadReadyDbContext _context;

        public PaymentsController(RoadReadyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto dto)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservationId);

            if (reservation == null)
            {
                return NotFound(new { message = "Reservation not found." });
            }

            if (reservation.Status != "Pending")
            {
                return BadRequest(new { message = "Payment has already been processed or reservation was cancelled." });
            }

            // Simulate credit card verification or PayPal API call
            var transactionId = "TXN-" + Guid.NewGuid().ToString().Substring(0, 18).ToUpper();

            var payment = new Payment
            {
                ReservationId = dto.ReservationId,
                Amount = reservation.TotalPrice,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                Status = "Success",
                TransactionId = transactionId
            };

            // Update Reservation Status and Car availability
            reservation.Status = "Confirmed";
            if (reservation.Car != null)
            {
                reservation.Car.IsAvailable = false;
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Payment processed successfully.",
                transactionId,
                paymentStatus = payment.Status,
                reservationStatus = reservation.Status
            });
        }
    }

    // DTO
    public class ProcessPaymentDto
    {
        public int ReservationId { get; set; }
        public string PaymentMethod { get; set; } = "CreditCard";
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
    }
}
