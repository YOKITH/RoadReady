using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadReady.API.DTOs;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        // ==========================================
        // Get All Maintenance Reports
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _maintenanceService.GetAllReportsAsync();

            return Ok(reports);
        }

        // ==========================================
        // Get Report By Id
        // ==========================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var report = await _maintenanceService.GetReportByIdAsync(id);

            if (report == null)
                return NotFound("Maintenance report not found.");

            return Ok(report);
        }

        // ==========================================
        // Get Reports By Car
        // ==========================================

        [HttpGet("car/{carId}")]
        public async Task<IActionResult> GetReportsByCar(int carId)
        {
            var reports =
                await _maintenanceService.GetReportsByCarIdAsync(carId);

            return Ok(reports);
        }

        // ==========================================
        // Get Reports By Rental Agent
        // ==========================================

        [Authorize(Roles = "RentalAgent,Admin")]
        [HttpGet("agent/{agentId}")]
        public async Task<IActionResult> GetReportsByAgent(int agentId)
        {
            var reports =
                await _maintenanceService.GetReportsByAgentIdAsync(agentId);

            return Ok(reports);
        }

        // ==========================================
        // Add Maintenance Report
        // ==========================================

        [Authorize(Roles = "RentalAgent")]
        [HttpPost]
        public async Task<IActionResult> AddReport(
            [FromBody] MaintenanceDto maintenanceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int agentId = int.Parse(userIdClaim.Value);

            var result =
                await _maintenanceService.AddReportAsync(
                    agentId,
                    maintenanceDto);

            if (!result)
                return BadRequest("Unable to create maintenance report.");

            return Ok(new
            {
                Message = "Maintenance report created successfully.",
                Status = "Pending"
            });
        }

        // ==========================================
        // Update Maintenance Status
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromQuery] string status)
        {
            var result =
                await _maintenanceService.UpdateReportStatusAsync(
                    id,
                    status);

            if (!result)
                return BadRequest();

            return Ok(new
            {
                Message = "Maintenance report updated successfully.",
                NewStatus = status
            });
        }

        // ==========================================
        // Delete Maintenance Report
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var result =
                await _maintenanceService.DeleteReportAsync(id);

            if (!result)
                return NotFound("Maintenance report not found.");

            return Ok(new
            {
                Message = "Maintenance report deleted successfully."
            });
        }
    }
}