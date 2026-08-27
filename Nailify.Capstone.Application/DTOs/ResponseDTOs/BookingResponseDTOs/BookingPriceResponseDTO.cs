namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class BookingPriceResponseDTO
    {
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalDuration { get; set; }
        public List<DiscountBreakdownDTO> DiscountBreakdown { get; set; } = new();
    }

    public class DiscountBreakdownDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; // "Promotion" or "Loyalty"
        public string AmountDisplay => $"-{Amount:N0}";
    }
}
