using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.DTOs
{
    public class ReviewDto
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;
    }
}