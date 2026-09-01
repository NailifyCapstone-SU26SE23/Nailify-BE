using Nailify.Capstone.Application.Common;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRatingRequestDTOs
{
    public class BookingRatingRequestParameters : PagingRequestParameters
    {
        public int PageNumber
        {
            get => PageIndex;
            set => PageIndex = value;
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Stars { get; set; }
        public bool? IsNegativeOnly { get; set; }
    }
}
