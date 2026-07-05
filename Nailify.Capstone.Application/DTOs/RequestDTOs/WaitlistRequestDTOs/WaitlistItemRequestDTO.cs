using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.WaitlistRequestDTOs
{
    public class WaitlistItemRequestDTO : IMapFrom<WaitlistItem>
    {
        public int? NailVariantId { get; set; }
        public Guid? ServiceId { get; set; }
        public int? CustomerNailId { get; set; }
        public int Quantity { get; set; } = 1;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<WaitlistItemRequestDTO, WaitlistItem>()
                   .IgnoreAllNonExisting();
        }
    }
}
