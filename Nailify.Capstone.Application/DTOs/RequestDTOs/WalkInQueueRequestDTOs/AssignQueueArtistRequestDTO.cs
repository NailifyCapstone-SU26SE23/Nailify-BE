using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs
{
    public class AssignQueueArtistRequestDTO : IMapFrom<WalkInQueue>
    {
        public Guid NailArtistId { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<AssignQueueArtistRequestDTO, WalkInQueue>()
                   .IgnoreAllNonExisting()
                   .ForMember(dest => dest.AssignedNailArtistId, opt => opt.MapFrom(src => src.NailArtistId));
        }
    }
}
