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

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class InterleavingOpportunityResponseDTO : IMapFrom<Booking>
    {
        public Guid CheckedInBookingId { get; set; }
        public Guid AssignedArtistId { get; set; }
        public bool CanStartImmediately { get; set; }
        public bool IsPassiveInterleaving { get; set; }
        public BookingClientType OverlappingClientType { get; set; } = BookingClientType.PreBooked;
        public string CurrentProcedureName { get; set; } = string.Empty;
        public int RemainingPasiveMinutes { get; set; }
        public string RecommendationMessage { get; set; } = string.Empty;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Booking, InterleavingOpportunityResponseDTO>()
                .ForMember(dest => dest.CheckedInBookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.AssignedArtistId, opt => opt.MapFrom(src => src.NailArtistId ?? Guid.Empty))
                .IgnoreAllNonExisting();
        }
    }
}
