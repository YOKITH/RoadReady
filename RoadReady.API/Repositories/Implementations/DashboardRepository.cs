using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.DTOs;
using RoadReady.API.Repositories.Interfaces;

namespace RoadReady.API.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardStatsAsync()
        {
            return new DashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),

                TotalCars = await _context.Cars.CountAsync(),
                TotalReservations = await _context.Reservations.CountAsync(),

                TotalPayments = await _context.Payments.CountAsync(),

                TotalReviews = await _context.Reviews.CountAsync(),
                AvailableCars = await _context.Cars.CountAsync(c => c.IsAvailable),

                BookedCars = await _context.Cars.CountAsync(c => !c.IsAvailable),

                TotalRevenue = await _context.Payments.Where(p => p.PaymentStatus == "Success").SumAsync(p => (decimal?)p.Amount) ?? 0
            };
        }
    }
}