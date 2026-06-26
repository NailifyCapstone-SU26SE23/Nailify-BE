using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingRatingResponseDTOs
{
    public class BookingRatingResponseDTO : IMapFrom<BookingRating>
    {
        public Guid BookingRatingId { get; set; }
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalonId { get; set; }
        public Guid? NailArtistId { get; set; }
        public int OverallScore { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public int? ServiceQuality { get; set; }
        public int? Punctuality { get; set; }
        public int? Cleanliness { get; set; }
        public bool IsUpdated { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingRating, BookingRatingResponseDTO>()
                .ForMember(dest => dest.SalonId, opt => opt.MapFrom(src => src.Booking.SalonId))
                .ForMember(dest => dest.NailArtistId, opt => opt.MapFrom(src => src.Booking.NailArtistId));
        }
    }
}
