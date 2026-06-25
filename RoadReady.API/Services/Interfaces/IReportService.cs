using RoadReady.API.DTOs;

namespace RoadReady.API.Services.Interfaces
{
    public interface IReportService
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