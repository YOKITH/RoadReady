using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;

namespace RoadReady.API.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations.Include(r => r.User).Include(r => r.Car).ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
        {
            return await _context.Reservations.Include(r => r.User).Include(r => r.Car).FirstOrDefaultAsync(r => r.ReservationId == reservationId);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public Task UpdateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            return Task.CompletedTask;
        }

        public async Task<bool> IsCarBookedAsync(int carId, DateTime pickupDate,DateTime dropoffDate)
        {
            return await _context.Reservations.AnyAsync(r =>
                r.CarId == carId &&
                r.Status != "Cancelled" &&
                pickupDate < r.DropoffDate &&
                dropoffDate > r.PickupDate);
        }

        public Task DeleteReservationAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResponse<Reservation>> GetPagedReservationsAsync(PaginationParams paginationParams)
        {
            var query = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Car)
                .AsQueryable();

            var totalRecords =
                await query.CountAsync();

            var reservations =await query.Skip(
                    (paginationParams.PageNumber - 1)
                    * paginationParams.PageSize)
                .Take(
                    paginationParams.PageSize)
                .ToListAsync();

            return new PagedResponse<Reservation>
            {
                Data = reservations,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)paginationParams.PageSize)
            };
        }
    }
}