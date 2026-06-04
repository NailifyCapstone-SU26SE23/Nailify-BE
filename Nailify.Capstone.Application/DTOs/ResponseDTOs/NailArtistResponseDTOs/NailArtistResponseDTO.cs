using AutoMapper;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ScheduleResponseDTOs;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs
{
    public class NailArtistResponseDTO : IMapFrom<NailArtist>
    {
        public Guid NailArtistId { get; set; }
        public Guid AccountId { get; set; }
        public Guid SalonId { get; set; }
        public string Status { get; set; } = string.Empty;

        // Account details
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;

        public List<ScheduleResponseDTO> Schedules { get; set; } = new List<ScheduleResponseDTO>();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<NailArtist, NailArtistResponseDTO>()
              .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Account.Email))
              .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.Account.FirstName))
              .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.Account.LastName))
              .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Account.Phone))
              .ForMember(d => d.AvatarUrl, opt => opt.MapFrom(s => s.Account.AvatarUrl));
        }
    }
}
