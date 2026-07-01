using AutoMapper;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services.Interfaces;
using Razorpay.Api;
using Razorpay.Api.Errors;
using System.Security.Cryptography;
using System.Text;

namespace RoadReady.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        private readonly IReservationRepository _reservationRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<PaymentService> _logger;

        private readonly IConfiguration _configuration;
        public PaymentService( IPaymentRepository paymentRepository,
            IReservationRepository reservationRepository, IMapper mapper,
            ILogger<PaymentService> logger, IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;

            _reservationRepository = reservationRepository;

            _mapper = mapper;

            _logger = logger;

            _configuration = configuration;
        }

        public async Task<IEnumerable<RoadReady.API.Models.Payment>> GetAllPaymentsAsync()
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

        public async Task<IEnumerable<RoadReady.API.Models.Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            try
            {
                return await _paymentRepository.GetPaymentsByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching payments for user {UserId}", userId);

                throw;
            }
        }

        public async Task<RoadReady.API.Models.Payment?> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);

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

        public async Task<PagedResponse<RoadReady.API.Models.Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams)
        {
            try
            {
                return await _paymentRepository.GetPagedPaymentsAsync(paginationParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching paged payments.");

                throw;
            }
        }

        public async Task<RazorpayOrderResponseDto> CreateRazorpayOrderAsync(int reservationId)
        {
            try
            {
                // Check whether reservation exists
                var reservation = await _reservationRepository
                    .GetReservationByIdAsync(reservationId);

                if (reservation == null)
                {
                    throw new KeyNotFoundException(
                        $"Reservation with ID {reservationId} not found.");
                }

                // Check if payment is already completed
                var existingPayment = await _paymentRepository
                    .GetPaymentByReservationIdAsync(reservationId);

                if (existingPayment != null &&
                    existingPayment.PaymentStatus == "Success")
                {
                    throw new InvalidOperationException(
                        "Payment has already been completed for this reservation.");
                }

                // Read Razorpay credentials
                string key = _configuration["Razorpay:Key"]!;

                string secret = _configuration["Razorpay:Secret"]!;

                // Create Razorpay client
                RazorpayClient client =
                    new RazorpayClient(key, secret);

                // Razorpay amount should be in paise
                Dictionary<string, object> options =
                    new Dictionary<string, object>();

                options.Add(
                    "amount",
                    Convert.ToInt32(reservation.TotalAmount * 100));

                options.Add(
                    "currency",
                    "INR");

                options.Add(
                    "receipt",
                    $"RR_{reservation.ReservationId}");

                options.Add(
                    "payment_capture",
                    1);

                // Create Order
                Order order =
                    client.Order.Create(options);

                string orderId = order["id"].ToString()!;

                _logger.LogInformation(
                    "Razorpay Order Created Successfully. ReservationId: {ReservationId}, OrderId: {OrderId}",
                    reservation.ReservationId,
                    orderId);

                // Return details to React
                return new RazorpayOrderResponseDto
                {
                    OrderId = order["id"].ToString()!,
                    Key = key,
                    Amount = reservation.TotalAmount,
                    Currency = "INR"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while creating Razorpay order.");

                throw;
            }
        }
        public async Task<bool> VerifyPaymentAsync(RazorpayPaymentDto paymentDto)
        {
            try
            {
                // Check Reservation
                var reservation = await _reservationRepository
                    .GetReservationByIdAsync(paymentDto.ReservationId);

                if (reservation == null)
                    throw new KeyNotFoundException("Reservation not found.");

                // Check Duplicate Payment
                var existingPayment = await _paymentRepository
                    .GetPaymentByRazorpayPaymentIdAsync(paymentDto.RazorpayPaymentId);

                if (existingPayment != null)
                    throw new InvalidOperationException("Payment already processed.");

                // Verify Razorpay Signature
                var secret = _configuration["Razorpay:Secret"];

                var payload =
                    paymentDto.RazorpayOrderId + "|" +
                    paymentDto.RazorpayPaymentId;

                using var hmac =
                    new HMACSHA256(
                        Encoding.UTF8.GetBytes(secret!));

                var hash =
                    hmac.ComputeHash(
                        Encoding.UTF8.GetBytes(payload));

                var generatedSignature =
                    Convert.ToHexString(hash).ToLower();

                if (generatedSignature !=
                    paymentDto.RazorpaySignature.ToLower())
                {
                    throw new UnauthorizedAccessException(
                        "Payment verification failed.");
                }

                // Create Payment
                var payment = new RoadReady.API.Models.Payment
                {
                    UserId = reservation.UserId,
                    ReservationId = reservation.ReservationId,
                    Amount = reservation.TotalAmount,
                    PaymentMethod = paymentDto.PaymentMethod,
                    PaymentStatus = "Success",
                    RazorpayOrderId = paymentDto.RazorpayOrderId,
                    RazorpayPaymentId = paymentDto.RazorpayPaymentId,
                    RazorpaySignature = paymentDto.RazorpaySignature,
                    PaymentDate = DateTime.UtcNow
                };

                await _paymentRepository
                    .AddPaymentAsync(payment);

                // Confirm Reservation
                reservation.Status = "Confirmed";

                await _reservationRepository
                    .UpdateReservationAsync(reservation);

                bool result =
                    await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Payment successful. ReservationId: {ReservationId}, PaymentId: {PaymentId}",
                    reservation.ReservationId,
                    payment.RazorpayPaymentId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while verifying Razorpay payment.");

                throw;
            }
        }




        private string GenerateSignature(string razorpayOrderId, string razorpayPaymentId)
        {
            string secret = _configuration["Razorpay:Secret"]!;

            string payload = $"{razorpayOrderId}|{razorpayPaymentId}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));

            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            return Convert.ToHexString(hash).ToLower();
        }
    }
}



























//using AutoMapper;
//using RoadReady.API.DTOs;
//using RoadReady.API.Models;
//using RoadReady.API.Pagination;
//using RoadReady.API.Repositories.Interfaces;
//using RoadReady.API.Services.Interfaces;

//namespace RoadReady.API.Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly IPaymentRepository _paymentRepository;
//        private readonly IReservationRepository _reservationRepository;
//        private readonly IMapper _mapper;
//        private readonly ILogger<PaymentService> _logger;

//        public PaymentService(IPaymentRepository paymentRepository,IReservationRepository reservationRepository,IMapper mapper,ILogger<PaymentService> logger)
//        {
//            _paymentRepository = paymentRepository;
//            _reservationRepository = reservationRepository;
//            _mapper = mapper;
//            _logger = logger;
//        }

//        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
//        {
//            try
//            {
//                return await _paymentRepository.GetAllPaymentsAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(
//                    ex,
//                    "Error occurred while fetching all payments.");

//                throw;
//            }
//        }

//        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
//        {
//            try
//            {
//                var payment =await _paymentRepository.GetPaymentByIdAsync(paymentId);

//                if (payment == null)
//                    throw new KeyNotFoundException(
//                        $"Payment with ID {paymentId} not found.");

//                return payment;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(
//                    ex,
//                    "Error occurred while fetching payment {PaymentId}",
//                    paymentId);

//                throw;
//            }
//        }

//        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
//        {
//            try
//            {
//                return await _paymentRepository.GetPaymentsByUserIdAsync(userId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex,"Error occurred while fetching payments for user {UserId}",userId);

//                throw;
//            }
//        }

//        public async Task<bool> ProcessPaymentAsync(PaymentDto paymentDto)
//        {
//            try
//            {
//                var reservation = await _reservationRepository.GetReservationByIdAsync(paymentDto.ReservationId);

//                if (reservation == null)
//                    throw new KeyNotFoundException(
//                        "Reservation not found.");

//                var payment = _mapper.Map<Payment>(paymentDto);

//                payment.UserId = reservation.UserId;

//                payment.Amount = reservation.TotalAmount;

//                payment.PaymentStatus = "Success";

//                payment.PaymentDate =DateTime.UtcNow;

//                reservation.Status = "Confirmed";

//                await _paymentRepository.AddPaymentAsync(payment);

//                await _reservationRepository.UpdateReservationAsync(reservation);

//                var result = await _paymentRepository.SaveChangesAsync();

//                _logger.LogInformation("Payment processed successfully for Reservation {ReservationId}",
//                    reservation.ReservationId);

//                return result;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex,"Error occurred while processing payment.");

//                throw;
//            }
//        }

//        public async Task<PagedResponse<Payment>> GetPagedPaymentsAsync(PaginationParams paginationParams)
//        {
//            try
//            {
//                return await _paymentRepository.GetPagedPaymentsAsync(paginationParams);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex,"Error occurred while fetching paged payments.");

//                throw;
//            }
//        }
//    }
//}