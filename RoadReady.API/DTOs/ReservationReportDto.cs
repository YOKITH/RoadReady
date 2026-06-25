namespace RoadReady.API.DTOs
{
    public class ReservationReportDto
    {
        public int TotalReservations { get; set; }

        public int ConfirmedReservations { get; set; }

        public int CancelledReservations { get; set; }

        public int PendingReservations { get; set; }
    }
}