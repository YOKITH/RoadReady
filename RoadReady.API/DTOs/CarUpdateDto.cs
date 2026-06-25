using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.DTOs
{
    public class CarUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }

        [Required]
        [Range(1, 100000)]
        public decimal PricePerDay { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}