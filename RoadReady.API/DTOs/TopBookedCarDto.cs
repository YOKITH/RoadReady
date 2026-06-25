namespace RoadReady.API.DTOs
{
    public class TopBookedCarDto
    {
        public int CarId { get; set; }

        public string CarName { get; set; } = string.Empty;

        public int TotalBookings { get; set; }
    }
}