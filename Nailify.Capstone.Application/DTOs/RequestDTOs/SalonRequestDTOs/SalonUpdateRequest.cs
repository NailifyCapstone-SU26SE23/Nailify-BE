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
    public class SalonUpdateRequest : IMapFrom<Salon>
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SalonUpdateRequest, Salon>()
                   .ForMember(dest => dest.SalonId, opt => opt.Ignore())
                   .ForMember(dest => dest.OperatingHours, opt => opt.Ignore());
        }
    }
}
