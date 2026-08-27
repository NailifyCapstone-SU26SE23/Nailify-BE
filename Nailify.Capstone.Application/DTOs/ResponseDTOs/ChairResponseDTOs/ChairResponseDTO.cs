using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.ChairResponseDTOs
{
    public class ChairResponseDTO : IMapFrom<Chair>
    {
        public Guid ChairId { get; set; }
        public Guid SalonId { get; set; }
        public string ChairName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? SalonName { get; set; }

        // Trạng thái bận/trống tại thời điểm query
        public bool IsOccupied { get; set; }
        public Guid? OccupiedByBookingId { get; set; }
        public Guid? OccupiedByCustomerId { get; set; }
        public string? OccupiedByCustomerName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Chair, ChairResponseDTO>()
                .ForMember(d => d.SalonName, opt => opt.MapFrom(s => s.Salon != null ? s.Salon.Name : null))
                .ForMember(d => d.IsOccupied, opt => opt.Ignore())
                .ForMember(d => d.OccupiedByBookingId, opt => opt.Ignore())
                .ForMember(d => d.OccupiedByCustomerId, opt => opt.Ignore())
                .ForMember(d => d.OccupiedByCustomerName, opt => opt.Ignore());
        }
    }
}
