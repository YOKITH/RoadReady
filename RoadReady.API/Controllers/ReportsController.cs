using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportsController: ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> RevenueReport()
        {
            return Ok(await _reportService.GetRevenueReportAsync());
        }

         
        [HttpGet("reservations")]
        public async Task<IActionResult> ReservationReport()
        {
            return Ok(await _reportService.GetReservationReportAsync());
        }


        [HttpGet("top-cars")]
        public async Task<IActionResult> TopBookedCars()
        {
            return Ok(await _reportService.GetTopBookedCarsAsync());
        }

        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> MonthlyRevenue()
        {
            return Ok(await _reportService.GetMonthlyRevenueAsync());
        }
    }
}