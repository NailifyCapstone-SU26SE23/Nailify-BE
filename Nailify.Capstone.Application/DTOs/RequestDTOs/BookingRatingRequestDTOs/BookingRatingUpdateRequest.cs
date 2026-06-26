namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs
{
    public class BookingRatingUpdateRequest
    {
        public int? OverallScore { get; set; }
        public string? Comment { get; set; }
        public int? ServiceQuality { get; set; }
        public int? Punctuality { get; set; }
        public int? Cleanliness { get; set; }
    }
}
