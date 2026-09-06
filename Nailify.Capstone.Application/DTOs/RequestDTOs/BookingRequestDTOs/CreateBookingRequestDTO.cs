using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class CreateBookingRequestDTO : IMapFrom<Booking>
    {
        public Guid SalonId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public Guid? NailArtistId { get; set; }
        public string? HoldToken { get; set; }
        public List<BookingItemRequestDTO> BookingItems { get; set; } = new();
        public List<int>? SelectedPromotionIds { get; set; }
        public Guid? WarrantyForBookingId { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateBookingRequestDTO, Booking>()
                .ForMember(dest => dest.BookingId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.TotalDuration, opt => opt.Ignore())
                .ForMember(dest => dest.CheckInImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CheckOutImagesUrl, opt => opt.Ignore())
                .ForMember(dest => dest.QRCode, opt => opt.Ignore())
                .ForMember(dest => dest.BookingItems, opt => opt.Ignore())
                .ForMember(dest => dest.WarrantyForBookingId, opt => opt.MapFrom(src => src.WarrantyForBookingId));
        }
    }
}
