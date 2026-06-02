using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs
{
    public class SalonOperatingHourUpdateRequest : IMapFrom<SalonOperatingHour>
    {
        public int DayOfWeek { get; set; }
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;
        public bool IsClosed { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SalonOperatingHourUpdateRequest, SalonOperatingHour>()
                .ForMember(dest => dest.OperatingHourId, opt => opt.Ignore())
                .ForMember(dest => dest.SalonId, opt => opt.Ignore())
                .ForMember(dest => dest.Salon, opt => opt.Ignore())
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.OpenTime)))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.CloseTime)));
        }
    }
}
