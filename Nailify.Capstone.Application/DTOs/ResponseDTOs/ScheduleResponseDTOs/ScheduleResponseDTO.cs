using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.ScheduleResponseDTOs
{
    public class ScheduleResponseDTO : IMapFrom<Schedule>
    {
        public Guid ScheduleId { get; set; }
        public Guid NailArtistId { get; set; }
        public DateTime WorkDate { get; set; }
        public string ShiftStart { get; set; } = string.Empty;
        public string ShiftEnd { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Schedule, ScheduleResponseDTO>()
                .ForMember(dest => dest.ShiftStart, opt => opt.MapFrom(src => src.ShiftStart.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.ShiftEnd, opt => opt.MapFrom(src => src.ShiftEnd.ToString(@"hh\:mm\:ss")));
        }
    }
}
