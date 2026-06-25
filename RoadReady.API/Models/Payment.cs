using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadReady.API.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
            public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending";
        // Pending, Success, Failed

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(ReservationId))]
        public Reservation Reservation { get; set; } = null!;
    }
}