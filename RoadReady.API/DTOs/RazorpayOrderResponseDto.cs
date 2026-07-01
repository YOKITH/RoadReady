namespace RoadReady.API.DTOs
{
    public class RazorpayOrderResponseDto
    {
        public string OrderId { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";
    }
}