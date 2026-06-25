using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class BookingItemResponseDTO : IMapFrom<BookingItem>
    {
        public Guid BookingItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int? NailVariantId { get; set; }
        public string NailVariantName { get; set; } = string.Empty;
        public string NailVariantImageUrl { get; set; } = string.Empty;
        public int? CustomerNailId { get; set; }
        public string CustomerNailName { get; set; } = string.Empty;
        public string CustomerNailImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<BookingItem, BookingItemResponseDTO>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : ""))
                .ForMember(dest => dest.NailVariantName, opt => opt.MapFrom(src => src.NailVariant != null ? src.NailVariant.Name : ""))
                .ForMember(dest => dest.NailVariantImageUrl, opt => opt.MapFrom(src => src.NailVariant != null ? src.NailVariant.ImageUrl : ""))
                .ForMember(dest => dest.CustomerNailName, opt => opt.MapFrom(src => src.CustomerNail != null ? src.CustomerNail.Name : ""))
                .ForMember(dest => dest.CustomerNailImageUrl, opt => opt.MapFrom(src => src.CustomerNail != null ? src.CustomerNail.ImageUrl : ""));
        }
    }
}
