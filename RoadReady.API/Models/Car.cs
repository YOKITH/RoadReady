using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = "Sedan"; // SUV, Sedan, Luxury, Hatchback, Electric

        [Required]
        [MaxLength(20)]
        public string Transmission { get; set; } = "Automatic"; // Automatic, Manual

        [Required]
        [MaxLength(20)]
        public string FuelType { get; set; } = "Petrol"; // Petrol, Diesel, Electric, Hybrid

        [Required]
        public int Seats { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
