using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Services.Interfaces;

namespace RoadReady.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            return Ok(await _paymentService.GetAllPaymentsAsync());
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUser(int userId)
        {
            var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);

            return Ok(payments);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromQuery] int reservationId)
        {
            var order =
                await _paymentService.CreateRazorpayOrderAsync(reservationId);

            return Ok(order);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] RazorpayPaymentDto paymentDto)
        {
            var result = await _paymentService.VerifyPaymentAsync(paymentDto);

            if (!result)
                return BadRequest("Payment verification failed.");

            return Ok(new
            {
                Message = "Payment Successful",
                ReservationStatus = "Confirmed"
            });
        }


        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedPayments([FromQuery] PaginationParams paginationParams)
        {
            return Ok(await _paymentService.GetPagedPaymentsAsync(paginationParams));
        }

    }
}








































//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using RoadReady.API.DTOs;
//using RoadReady.API.Models;
//using RoadReady.API.Pagination;
//using RoadReady.API.Services.Interfaces;

//namespace RoadReady.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class PaymentsController : ControllerBase
//    {
//        private readonly IPaymentService _paymentService;

//        public PaymentsController(IPaymentService paymentService)
//        {
//            _paymentService = paymentService;
//        }


//        [Authorize(Roles = "Admin")]
//        [HttpGet]
//        public async Task<IActionResult> GetAllPayments()
//        {
//            return Ok(await _paymentService.GetAllPaymentsAsync());
//        }



//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetPaymentById(int id)
//        {
//            var payment = await _paymentService.GetPaymentByIdAsync(id);

//            if (payment == null)
//                return NotFound();

//            return Ok(payment);
//        }


//        [HttpGet("user/{userId}")]
//        public async Task<IActionResult> GetPaymentsByUser(int userId)
//        {
//            var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);

//            return Ok(payments);
//        }

//        [HttpPost]
//        public async Task<IActionResult> ProcessPayment(
//            PaymentDto payment)
//        {
//            var result = await _paymentService.ProcessPaymentAsync(payment);

//            if (!result)
//                return BadRequest();

//            return Ok(new
//            {
//                Message = "Payment processed successfully.",
//                ReservationStatus = "Confirmed",
//                Note = "Your selected car has been successfully reserved for the chosen travel period. Please arrive at the pickup location on the scheduled date and time."
//            });
//        }


//        [HttpGet("paged")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetPagedPayments([FromQuery] PaginationParams paginationParams)
//        {
//            return Ok(await _paymentService.GetPagedPaymentsAsync(paginationParams));
//        }

//    }
//}