using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.DTOs
{
    public class PaymentDto
    {
        [Required]
        public int ReservationId { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
        [Required]
        public int Amount { get; set; }
        [Required]
        public string PaymentStatus { get; set; }
    }
}