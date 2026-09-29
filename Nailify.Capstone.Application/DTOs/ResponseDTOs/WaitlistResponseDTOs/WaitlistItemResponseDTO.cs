using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.WaitlistResponseDTOs
{
    public class WaitlistItemResponseDTO : IMapFrom<WaitlistItem>
    {
        public Guid WaitlistItemId { get; set; }
        public Guid WaitlistId { get; set; }
        public int? NailVariantId { get; set; }
        public string NailVariantName { get; set; } = string.Empty;
        public string NailVariantImageUrl { get; set; } = string.Empty;
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int? CustomerNailId { get; set; }
        public string CustomerNailName { get; set; } = string.Empty;
        public string CustomerNailImageUrl { get; set; } = string.Empty;
        public int? ShapeMethodConfigId { get; set; }
        public string ShapeMethodConfigName { get; set; } = string.Empty;
        public Guid? CustomerNailRequestId { get; set; }
        public int Quantity { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<WaitlistItem, WaitlistItemResponseDTO>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
                .ForMember(dest => dest.NailVariantName, opt => opt.MapFrom(src => src.NailVariant != null ? src.NailVariant.Name : string.Empty))
                .ForMember(dest => dest.NailVariantImageUrl, opt => opt.MapFrom(src => src.NailVariant != null ? src.NailVariant.ImageUrl : string.Empty))
                .ForMember(dest => dest.ShapeMethodConfigName, opt => opt.MapFrom(src => src.ShapeMethodConfig != null ? src.ShapeMethodConfig.Name : string.Empty))
                .ForMember(dest => dest.CustomerNailId, opt => opt.MapFrom(src => src.CustomerNailId ?? (src.CustomerNailRequest != null ? src.CustomerNailRequest.CustomerNailId : (int?)null)))
                .ForMember(dest => dest.CustomerNailName, opt => opt.MapFrom(src => src.CustomerNail != null ? src.CustomerNail.Name : (src.CustomerNailRequest != null && src.CustomerNailRequest.CustomerNail != null ? src.CustomerNailRequest.CustomerNail.Name : string.Empty)))
                .ForMember(dest => dest.CustomerNailImageUrl, opt => opt.MapFrom(src => src.CustomerNail != null ? src.CustomerNail.ImageUrl : (src.CustomerNailRequest != null && src.CustomerNailRequest.CustomerNail != null ? src.CustomerNailRequest.CustomerNail.ImageUrl : string.Empty)));
        }
    }
}
