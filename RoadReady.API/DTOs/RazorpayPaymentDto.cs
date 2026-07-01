using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.DTOs
{
    public class RazorpayPaymentDto
    {
        [Required]
        public int ReservationId { get; set; }

        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required]
        public string RazorpaySignature { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}