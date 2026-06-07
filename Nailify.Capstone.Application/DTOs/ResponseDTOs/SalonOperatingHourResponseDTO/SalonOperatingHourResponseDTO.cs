using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonOperatingHourResponseDTO
{
    public class SalonOperatingHourResponseDTO : IMapFrom<SalonOperatingHour>
    {
        public int DayOfWeek { get; set; }
        /// <summary>
        /// 0 = Sunday, 1 = Monday, ..., 6 = Saturday
        /// </summary>
        public string DayName => ((System.DayOfWeek)this.DayOfWeek).ToString();
        public string OpenTime { get; set; } = string.Empty; // hh:mm:ss
        public string CloseTime { get; set; } = string.Empty; // hh:mm:ss
        public bool IsClosed { get; set; }

        // Cau hinh AutoMapper de chuyen doi TimeSpan sang string
        public void Mapping(Profile profile)
        {
            profile.CreateMap<SalonOperatingHour, SalonOperatingHourResponseDTO>()
                .ForMember(dest => dest.OpenTime,
                    opt => opt.MapFrom(src => src.OpenTime.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.CloseTime,
                    opt => opt.MapFrom(src => src.CloseTime.ToString(@"hh\:mm\:ss")));
        }
    }
}
