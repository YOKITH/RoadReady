//using RoadReady.API.DTOs;
//using RoadReady.API.Models;
//using RoadReady.API.Pagination;

//namespace RoadReady.API.Services.Interfaces
//{
//    public interface IPaymentService
//    {
//        Task<IEnumerable<Payment>> GetAllPaymentsAsync();

//        Task<Payment?> GetPaymentByIdAsync(int paymentId);

//        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);

//        Task<bool> ProcessPaymentAsync(PaymentDto payment);

//        Task<PagedResponse<Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams);
//    }
//}




using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;

namespace RoadReady.API.Services.Interfaces
{
    public interface IPaymentService
    {
        // Get all payments
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();

        // Get payment by ID
        Task<Payment?> GetPaymentByIdAsync(int paymentId);

        // Get payments by User ID
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);

        // Create Razorpay Order
        Task<RazorpayOrderResponseDto> CreateRazorpayOrderAsync(int reservationId);

        // Verify Razorpay Payment and Confirm Reservation
        Task<bool> VerifyPaymentAsync(RazorpayPaymentDto paymentDto);

        // Get paginated payments
        Task<PagedResponse<Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams);
    }
}