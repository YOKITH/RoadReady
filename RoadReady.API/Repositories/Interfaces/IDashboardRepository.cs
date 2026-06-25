using RoadReady.API.DTOs;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardDto> GetDashboardStatsAsync();
    }
}