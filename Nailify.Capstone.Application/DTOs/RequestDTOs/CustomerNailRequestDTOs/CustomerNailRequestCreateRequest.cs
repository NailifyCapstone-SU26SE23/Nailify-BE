using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs
{
    public class CustomerNailRequestCreateRequest : IMapFrom<CustomerNailRequest>
    {
        public int CustomerNailId { get; set; }
        public Guid SalonId { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerNailRequestCreateRequest, CustomerNailRequest>()
                   .IgnoreAllNonExisting()
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CustomerNailStatus.PendingReview))
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
