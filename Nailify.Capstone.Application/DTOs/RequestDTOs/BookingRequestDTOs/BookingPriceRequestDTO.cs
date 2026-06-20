namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class BookingPriceRequestDTO
    {
        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
    }
}
