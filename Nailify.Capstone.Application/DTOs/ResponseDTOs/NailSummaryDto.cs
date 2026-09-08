namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailSummaryDto
    {
        public int TotalBookings { get; set; }
        public int TotalFavorites { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}
