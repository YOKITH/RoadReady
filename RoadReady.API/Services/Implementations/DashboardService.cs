using RoadReady.API.DTOs;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IDashboardRepository dashboardRepository,ILogger<DashboardService> logger)
        {
            _dashboardRepository = dashboardRepository;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardStatsAsync()
        {
            try
            {
                var dashboardStats = await _dashboardRepository.GetDashboardStatsAsync();

                if (dashboardStats == null)
                    throw new KeyNotFoundException(
                        "Dashboard statistics not found.");

                _logger.LogInformation("Dashboard statistics retrieved successfully.");

                return dashboardStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching dashboard statistics.");

                throw;
            }
        }
    }
}