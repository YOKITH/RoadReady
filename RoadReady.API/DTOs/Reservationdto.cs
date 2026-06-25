using System.ComponentModel.DataAnnotations;

namespace RoadReady.API.DTOs
{
    public class Reservationdto
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DropoffDate { get; set; }
    }
}