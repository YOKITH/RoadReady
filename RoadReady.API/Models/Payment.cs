using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadReady.API.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string PaymentMethod { get; set; } = "CreditCard"; // CreditCard, PayPal, UPI

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        [Required]
        [MaxLength(100)]
        public string TransactionId { get; set; } = string.Empty;
    }
}
