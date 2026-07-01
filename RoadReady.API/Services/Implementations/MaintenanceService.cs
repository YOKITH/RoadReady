using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MaintenanceService> _logger;

        public MaintenanceService(
            IMaintenanceRepository maintenanceRepository,
            ICarRepository carRepository,
            IMapper mapper,
            ILogger<MaintenanceService> logger)
        {
            _maintenanceRepository = maintenanceRepository;
            _carRepository = carRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<MaintenanceReport>> GetAllReportsAsync()
        {
            try
            {
                return await _maintenanceRepository.GetAllReportsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching maintenance reports.");

                throw;
            }
        }

        public async Task<MaintenanceReport?> GetReportByIdAsync(int reportId)
        {
            try
            {
                var report = await _maintenanceRepository.GetReportByIdAsync(reportId);

                if (report == null)
                    throw new KeyNotFoundException(
                        $"Maintenance report with ID {reportId} not found.");

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching maintenance report {ReportId}.",
                    reportId);

                throw;
            }
        }

        public async Task<IEnumerable<MaintenanceReport>> GetReportsByCarIdAsync(int carId)
        {
            try
            {
                return await _maintenanceRepository.GetReportsByCarIdAsync(carId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching reports for Car {CarId}.",
                    carId);

                throw;
            }
        }

        public async Task<IEnumerable<MaintenanceReport>> GetReportsByAgentIdAsync(int agentId)
        {
            try
            {
                return await _maintenanceRepository.GetReportsByAgentIdAsync(agentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching reports for Agent {AgentId}.",
                    agentId);

                throw;
            }
        }

        public async Task<bool> AddReportAsync(
            int agentId,
            MaintenanceDto maintenanceDto)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(
                    maintenanceDto.CarId);

                if (car == null)
                    throw new KeyNotFoundException(
                        "Car not found.");

                var report = _mapper.Map<MaintenanceReport>(
                    maintenanceDto);

                report.ReportedBy = agentId;
                report.ReportedDate = DateTime.UtcNow;
                report.Status = "Pending";

                // Vehicle goes to maintenance
                car.Status = "Maintenance";
                car.IsAvailable = false;

                await _maintenanceRepository.AddReportAsync(report);

                await _carRepository.UpdateCarAsync(car);

                var result = await _maintenanceRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Maintenance report created successfully. CarId: {CarId}",
                    maintenanceDto.CarId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while creating maintenance report.");

                throw;
            }
        }

        public async Task<bool> UpdateReportStatusAsync(
            int reportId,
            string status)
        {
            try
            {
                var report = await _maintenanceRepository.GetReportByIdAsync(
                    reportId);

                if (report == null)
                    throw new KeyNotFoundException(
                        "Maintenance report not found.");

                report.Status = status;

                if (status == "Completed")
                {
                    report.ResolvedDate = DateTime.UtcNow;

                    var car = await _carRepository.GetCarByIdAsync(report.CarId);

                    if (car != null)
                    {
                        car.Status = "Available";
                        car.IsAvailable = true;

                        await _carRepository.UpdateCarAsync(car);
                    }
                }

                await _maintenanceRepository.UpdateReportAsync(report);

                var result =
                    await _maintenanceRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Maintenance report updated successfully. ReportId: {ReportId}",
                    reportId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while updating maintenance report.");

                throw;
            }
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            try
            {
                var report =
                    await _maintenanceRepository.GetReportByIdAsync(reportId);

                if (report == null)
                    throw new KeyNotFoundException(
                        "Maintenance report not found.");

                await _maintenanceRepository.DeleteReportAsync(report);

                var result =
                    await _maintenanceRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Maintenance report deleted successfully. ReportId: {ReportId}",
                    reportId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while deleting maintenance report.");

                throw;
            }
        }
    }
}