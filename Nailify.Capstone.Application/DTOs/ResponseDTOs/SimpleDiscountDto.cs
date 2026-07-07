namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class SimpleDiscountDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AmountDisplay => $"-{Amount:N0}d";
        public string Type { get; set; } = string.Empty;
    }
}
