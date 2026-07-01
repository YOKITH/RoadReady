using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadReady.API.Models
{
    public class MaintenanceReport
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public int ReportedBy { get; set; }   // Rental Agent (UserId)

        [Required]
        [StringLength(100)]
        public string IssueType { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";
        // Pending, In Progress, Completed

        public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedDate { get; set; }

        // Navigation Properties

        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; } = null!;

        [ForeignKey(nameof(ReportedBy))]
        public User ReportedByUser { get; set; } = null!;
    }
}