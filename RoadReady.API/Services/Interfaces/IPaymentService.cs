using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();

        Task<Payment?> GetPaymentByIdAsync(int paymentId);

        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);

        Task<bool> ProcessPaymentAsync(PaymentDto payment);

        Task<PagedResponse<Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams);
    }
}