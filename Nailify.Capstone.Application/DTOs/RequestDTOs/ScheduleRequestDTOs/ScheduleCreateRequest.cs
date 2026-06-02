using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ScheduleRequestDTOs
{
    public class ScheduleCreateRequest : IMapFrom<Schedule>
    {
        public Guid NailArtistId { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public string Status { get; set; } = "Active";

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ScheduleCreateRequest, Schedule>()
                   .ForMember(dest => dest.ScheduleId, opt => opt.Ignore())
                   .ForMember(dest => dest.NailArtist, opt => opt.Ignore());
        }
    }
}
