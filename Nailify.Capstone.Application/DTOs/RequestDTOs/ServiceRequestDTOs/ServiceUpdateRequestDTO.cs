using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.ServiceRequestDTOs
{
    public class ServiceUpdateRequestDTO : IMapFrom<Nailify.Capstone.Domain.Entities.Services>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ServiceUpdateRequestDTO, Nailify.Capstone.Domain.Entities.Services>()
                   .IgnoreAllNonExisting();
        }
    }
}
