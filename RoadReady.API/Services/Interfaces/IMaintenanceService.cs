using RoadReady.API.DTOs;
using RoadReady.API.Models;

namespace RoadReady.API.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceReport>> GetAllReportsAsync();

        Task<MaintenanceReport?> GetReportByIdAsync(int reportId);

        Task<IEnumerable<MaintenanceReport>> GetReportsByCarIdAsync(int carId);

        Task<IEnumerable<MaintenanceReport>> GetReportsByAgentIdAsync(int agentId);

        Task<bool> AddReportAsync(int agentId, MaintenanceDto maintenanceDto);

        Task<bool> UpdateReportStatusAsync(int reportId, string status);

        Task<bool> DeleteReportAsync(int reportId);
    }
}