using Microsoft.EntityFrameworkCore;
using RoadReady.API.Data;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;

namespace RoadReady.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments.Include(p => p.User).Include(p => p.Reservation)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments.Include(p => p.User)
                .Include(p => p.Reservation)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public Task DeletePaymentAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<PagedResponse<Payment>> GetPagedPaymentsAsync (PaginationParams paginationParams)
        {
            var query = _context.Payments.Include(p => p.User)
                .Include(p => p.Reservation).AsQueryable();

            var totalRecords = await query.CountAsync();

            var payments = await query
                .Skip((paginationParams.PageNumber - 1)* paginationParams.PageSize)
                .Take(paginationParams.PageSize).ToListAsync();

            return new PagedResponse<Payment>
            {
                Data = payments,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalRecords = totalRecords,
                TotalPages =(int)Math.Ceiling(totalRecords /(double)paginationParams.PageSize)
            };
        }
    }
}