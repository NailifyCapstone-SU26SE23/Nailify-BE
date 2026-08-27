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
    public class UpdateSalonOffDateRequestDTO : IMapFrom<SalonOffDate>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateSalonOffDateRequestDTO, SalonOffDate>()
                   .ForMember(dest => dest.StartDate, opt => opt.Ignore()) 
                   .ForMember(dest => dest.EndDate, opt => opt.Ignore())
                   .IgnoreAllNonExisting();
        }
    }
}
