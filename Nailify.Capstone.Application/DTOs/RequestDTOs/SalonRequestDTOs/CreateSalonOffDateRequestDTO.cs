using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs
{
    public class CreateSalonOffDateRequestDTO : IMapFrom<SalonOffDate>
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateSalonOffDateRequestDTO, SalonOffDate>()
                   .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.Date))
                   .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.Date : src.StartDate.Date))
                   .IgnoreAllNonExisting();
        }
    }
}
