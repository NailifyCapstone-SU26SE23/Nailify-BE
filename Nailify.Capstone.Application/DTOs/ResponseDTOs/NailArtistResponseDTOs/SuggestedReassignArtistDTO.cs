using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs
{
    public class SuggestedReassignArtistDTO : IMapFrom<NailArtist>
    {
        public Guid NailArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public int SkillMatchLevel { get; set; }
        public bool IsFullyAvailable { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtist, SuggestedReassignArtistDTO>()
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Account != null ? $"{src.Account.FirstName} {src.Account.LastName}" : "Thợ nail"))
                .IgnoreAllNonExisting();
        }
    }
}
