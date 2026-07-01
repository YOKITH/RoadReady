using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.DTOs
{
    public class MaintenanceDto
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        [StringLength(100)]
        public string IssueType { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}