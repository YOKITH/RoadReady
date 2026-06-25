namespace RoadReady.API.DTOs
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }

        public int TotalCars { get; set; }

        public int TotalReservations { get; set; }

        public int TotalPayments { get; set; }

        public int TotalReviews { get; set; }

        public int AvailableCars { get; set; }

        public int BookedCars { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}