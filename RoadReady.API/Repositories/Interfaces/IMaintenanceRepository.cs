using RoadReady.API.Models;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task<IEnumerable<MaintenanceReport>> GetAllReportsAsync();

        Task<MaintenanceReport?> GetReportByIdAsync(int reportId);

        Task<IEnumerable<MaintenanceReport>> GetReportsByCarIdAsync(int carId);

        Task<IEnumerable<MaintenanceReport>> GetReportsByAgentIdAsync(int agentId);

        Task AddReportAsync(MaintenanceReport report);

        Task UpdateReportAsync(MaintenanceReport report);

        Task DeleteReportAsync(MaintenanceReport report);

        Task<bool> SaveChangesAsync();
    }
}