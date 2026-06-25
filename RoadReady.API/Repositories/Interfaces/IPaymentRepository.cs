using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();

        Task<Payment?> GetPaymentByIdAsync(int paymentId);

        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);

        Task AddPaymentAsync(Payment payment);

        Task UpdatePaymentAsync(Payment payment);

        Task DeletePaymentAsync(Payment payment);

        Task<bool> SaveChangesAsync();

        Task<PagedResponse<Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams);
    }
}