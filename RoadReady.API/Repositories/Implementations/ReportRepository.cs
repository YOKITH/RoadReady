using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.DTOs;
using RoadReady.API.Repositories.Interfaces;

namespace RoadReady.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync()
        {
            return new RevenueReportDto
            {
                TotalRevenue =await _context.Payments.Where(x =>x.PaymentStatus ==
                   "Success").SumAsync(x =>(decimal?)x.Amount) ?? 0,

                TotalPayments = await _context.Payments.CountAsync()
            };
        }

        public async Task<ReservationReportDto> GetReservationReportAsync()
        {
            return new ReservationReportDto
            {
                TotalReservations = await _context.Reservations.CountAsync(),

                ConfirmedReservations = await _context.Reservations.CountAsync(r => r.Status =="Confirmed"),

                CancelledReservations = await _context.Reservations.CountAsync(r =>r.Status =="Cancelled"),

                PendingReservations =await _context.Reservations.CountAsync(r =>
              r.Status =="Pending")
            };
        }

        public async Task<IEnumerable<TopBookedCarDto>> GetTopBookedCarsAsync()
        {
            return await _context.Reservations
                .GroupBy(r => new
                {
                    r.CarId,
                    r.Car.Brand,
                    r.Car.Model
                })
                .Select(g =>
                    new TopBookedCarDto
                    {
                        CarId = g.Key.CarId,
                        CarName = g.Key.Brand + " " + g.Key.Model,
                        TotalBookings = g.Count()
                    })
                .OrderByDescending(
                    x => x.TotalBookings) .Take(5).ToListAsync();
        }

        public async Task<IEnumerable<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            return await _context.Payments
                .Where(p =>
                    p.PaymentStatus ==
                    "Success").GroupBy(p =>
                    p.PaymentDate.Month)
                .Select(g =>
                    new MonthlyRevenueDto
                    {
                        Month = g.Key,
                        Revenue = g.Sum(x =>x.Amount)
                    })
                .OrderBy(x => x.Month)
                .ToListAsync();
        }
    }
}