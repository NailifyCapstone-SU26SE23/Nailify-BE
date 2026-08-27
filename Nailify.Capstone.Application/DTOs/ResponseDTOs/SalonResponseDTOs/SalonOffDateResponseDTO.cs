using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs
{
    public class SalonOffDateResponseDTO : IMapFrom<SalonOffDate>
    {
        public Guid SalonOffDateId { get; set; }
        public Guid SalonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<SalonOffDate, SalonOffDateResponseDTO>()
                   .IgnoreAllNonExisting();
        }
    }
}
