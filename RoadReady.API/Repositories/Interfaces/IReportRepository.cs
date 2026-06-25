using RoadReady.API.DTOs;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<RevenueReportDto>
            GetRevenueReportAsync();

        Task<ReservationReportDto>
            GetReservationReportAsync();

        Task<IEnumerable<TopBookedCarDto>>
            GetTopBookedCarsAsync();

        Task<IEnumerable<MonthlyRevenueDto>>
            GetMonthlyRevenueAsync();
    }
}