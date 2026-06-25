namespace RoadReady.API.DTOs
{
    public class CarCreateDto
    {
        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public decimal PricePerDay { get; set; }

        public string Location { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }
    }

}