using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            return Ok(await _reservationService.GetAllReservationsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

         
        [HttpGet( "user/{userId}")]
        public async Task<IActionResult> GetUserReservations(int userId)
        {
            var reservations = await _reservationService.GetReservationsByUserIdAsync(userId);

            return Ok(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(Reservationdto reservation)
        {
            var result = await _reservationService.CreateReservationAsync(reservation);

            return Ok(new
            {
                Message = "Reservation created successfully",
                Reservation = result
            });
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var result = await _reservationService.CancelReservationAsync(id);

            if (!result)
                return NotFound();

            return Ok( "Reservation cancelled successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedReservations([FromQuery] PaginationParams paginationParams)
        {
            var reservations = await _reservationService.GetPagedReservationsAsync(paginationParams);

            return Ok(reservations);
        }
    }
}