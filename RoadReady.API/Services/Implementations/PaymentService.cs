using AutoMapper;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPaymentRepository paymentRepository,IReservationRepository reservationRepository,IMapper mapper,ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            try
            {
                return await _paymentRepository.GetAllPaymentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching all payments.");

                throw;
            }
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment =await _paymentRepository.GetPaymentByIdAsync(paymentId);

                if (payment == null)
                    throw new KeyNotFoundException(
                        $"Payment with ID {paymentId} not found.");

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching payment {PaymentId}",
                    paymentId);

                throw;
            }
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            try
            {
                return await _paymentRepository.GetPaymentsByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching payments for user {UserId}",userId);

                throw;
            }
        }

        public async Task<bool> ProcessPaymentAsync(PaymentDto paymentDto)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(paymentDto.ReservationId);

                if (reservation == null)
                    throw new KeyNotFoundException(
                        "Reservation not found.");

                var payment = _mapper.Map<Payment>(paymentDto);

                payment.UserId = reservation.UserId;

                payment.Amount = reservation.TotalAmount;

                payment.PaymentStatus = "Success";

                payment.PaymentDate =DateTime.UtcNow;

                reservation.Status = "Confirmed";

                await _paymentRepository.AddPaymentAsync(payment);

                await _reservationRepository.UpdateReservationAsync(reservation);

                var result = await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation("Payment processed successfully for Reservation {ReservationId}",
                    reservation.ReservationId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while processing payment.");

                throw;
            }
        }

        public async Task<PagedResponse<Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams)
        {
            try
            {
                return await _paymentRepository.GetPagedPaymentsAsync(paginationParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred while fetching paged payments.");

                throw;
            }
        }
    }
}