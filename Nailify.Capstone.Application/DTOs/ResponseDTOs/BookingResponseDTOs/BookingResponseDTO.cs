using AutoMapper;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class BookingResponseDTO : IMapFrom<Booking>
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid SalonId { get; set; }
        public string SalonName { get; set; } = string.Empty;
        public Guid? NailArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalDuration { get; set; }
        public string? CheckInImageUrl { get; set; }
        public string? CheckOutImagesUrl { get; set; }
        public string? QRCode { get; set; }
        public DateTime? ActualCheckInTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public bool IsLateArrival { get; set; }
        public bool IsRated { get; set; }
        public bool IsPaid { get; set; }
        public bool IsRefunded { get; set; }
        public List<BookingItemResponseDTO> BookingItems { get; set; } = new();
        public List<SimpleDiscountDto> Discounts { get; set; } = new();
        public void Mapping(Profile profile)
        {
            // Tự động map các mối quan hệ lồng ghép và tên
            profile.CreateMap<Booking, BookingResponseDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.User.FirstName + " " + src.Customer.User.LastName))
                .ForMember(dest => dest.SalonName, opt => opt.MapFrom(src => src.Salon.Name))
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.NailArtist != null ? src.NailArtist.Account.FirstName + " " + src.NailArtist.Account.LastName : "Chưa chỉ định"))
                .ForMember(dest => dest.BookingItems, opt => opt.MapFrom(src => src.BookingItems))
                .ForMember(dest => dest.Discounts, opt => opt.MapFrom(src => src.BookingDiscounts
                    .Select(discount => new SimpleDiscountDto
                    {
                        Name = discount.Name,
                        Amount = discount.DiscountAmount,
                        Type = discount.PromotionId.HasValue ? "Promotion" : "Loyalty"
                    })));
        }
    }
}
