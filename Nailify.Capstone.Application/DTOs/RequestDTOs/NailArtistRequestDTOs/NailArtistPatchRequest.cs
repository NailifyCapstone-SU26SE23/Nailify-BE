using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs
{
    public class NailArtistPatchRequest : IMapFrom<NailArtist>
    {
        public Guid? SalonId { get; set; }
        public string? Status { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtistPatchRequest, NailArtist>()
                   .ForMember(dest => dest.NailArtistId, opt => opt.Ignore())
                   .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                   .ForMember(dest => dest.Account, opt => opt.Ignore())
                   .ForMember(dest => dest.Salon, opt => opt.Ignore())
                   .ForMember(dest => dest.Schedules, opt => opt.Ignore())
                   .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
