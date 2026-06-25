using RoadReady.API.DTOs;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class ReportService: IReportService
    {
        private readonly IReportRepository _reportRepository;

        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository reportRepository,ILogger<ReportService> logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync()
        {
            try
            {
                return await _reportRepository.GetRevenueReportAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Revenue report failed.");

                throw;
            }
        }

        public async Task<ReservationReportDto> GetReservationReportAsync()
        {
            try
            {
                return await _reportRepository.GetReservationReportAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Reservation report failed.");

                throw;
            }
        }

        public async Task<IEnumerable<TopBookedCarDto>> GetTopBookedCarsAsync()
        {
            try
            {
                return await _reportRepository.GetTopBookedCarsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Top booked cars report failed.");

                throw;
            }
        }

        public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            try
            {
                return await _reportRepository.GetMonthlyRevenueAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Monthly revenue report failed.");

                throw;
            }
        }
    }
}