using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using RoadReady.API.Repositories.Interfaces;

namespace RoadReady.API.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly AppDbContext _context;

        public MaintenanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceReport>> GetAllReportsAsync()
        {
            return await _context.MaintenanceReports
                .Include(m => m.Car)
                .Include(m => m.ReportedByUser)
                .ToListAsync();
        }

        public async Task<MaintenanceReport?> GetReportByIdAsync(int reportId)
        {
            return await _context.MaintenanceReports
                .Include(m => m.Car)
                .Include(m => m.ReportedByUser)
                .FirstOrDefaultAsync(m => m.ReportId == reportId);
        }

        public async Task<IEnumerable<MaintenanceReport>> GetReportsByCarIdAsync(int carId)
        {
            return await _context.MaintenanceReports
                .Include(m => m.ReportedByUser)
                .Where(m => m.CarId == carId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceReport>> GetReportsByAgentIdAsync(int agentId)
        {
            return await _context.MaintenanceReports
                .Include(m => m.Car)
                .Where(m => m.ReportedBy == agentId)
                .ToListAsync();
        }

        public async Task AddReportAsync(MaintenanceReport report)
        {
            await _context.MaintenanceReports.AddAsync(report);
        }

        public Task UpdateReportAsync(MaintenanceReport report)
        {
            _context.MaintenanceReports.Update(report);

            return Task.CompletedTask;
        }

        public Task DeleteReportAsync(MaintenanceReport report)
        {
            _context.MaintenanceReports.Remove(report);

            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}