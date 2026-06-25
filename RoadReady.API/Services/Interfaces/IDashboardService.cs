using RoadReady.API.DTOs;

namespace RoadReady.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardStatsAsync();
    }
}