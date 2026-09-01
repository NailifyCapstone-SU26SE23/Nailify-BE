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
    public class SalonPatchRequest : IMapFrom<Salon>
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Status { get; set; }
        public decimal DepositConfig { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SalonPatchRequest, Salon>()
                   .ForMember(dest => dest.SalonId, opt => opt.Ignore())
                   .ForMember(dest => dest.OperatingHours, opt => opt.Ignore())
                   .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
