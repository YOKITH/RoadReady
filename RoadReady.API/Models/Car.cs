using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
            = new List<Reservation>();

        public ICollection<Review> Reviews { get; set; }
            = new List<Review>();
    }
}