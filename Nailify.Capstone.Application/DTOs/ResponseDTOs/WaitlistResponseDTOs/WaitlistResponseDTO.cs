using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.WaitlistResponseDTOs
{
    public class WaitlistResponseDTO : IMapFrom<BookingWaitlist>
    {
        public Guid WailistId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid SalonId { get; set; }
        public string SalonName { get; set; } = string.Empty;
        public Guid? PreferredNailArtistId { get; set; }
        public string PreferredNailArtistName { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public TimeSpan RequestedStartTime { get; set; }
        public int EstimatedDuration { get; set; }
        public int Position { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public Guid? ConvertedBookingId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingWaitlist, WaitlistResponseDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.User != null ? src.Customer.User.FirstName + " " + src.Customer.User.LastName : string.Empty))
                .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src => src.Salon != null ? src.Salon.Name : string.Empty))
                .ForMember(dest => dest.PreferredNailArtistName, opt => opt.MapFrom(src => src.PreferredNailArtist != null && src.PreferredNailArtist.Account != null ? src.PreferredNailArtist.Account.FirstName + " " + src.PreferredNailArtist.Account.LastName : "Chưa chỉ định"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
