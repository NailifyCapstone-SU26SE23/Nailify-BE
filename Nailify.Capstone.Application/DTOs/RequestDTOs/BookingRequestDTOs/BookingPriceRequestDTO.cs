namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class BookingPriceRequestDTO
    {
        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
        public List<int>? SelectedPromotionIds { get; set; }    
        public Guid? WarrantyForBookingId { get; set; }
    }
}
