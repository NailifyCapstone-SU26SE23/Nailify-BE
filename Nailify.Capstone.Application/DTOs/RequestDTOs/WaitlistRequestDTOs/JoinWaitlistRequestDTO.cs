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
    public class JoinWaitlistRequestDTO : IMapFrom<BookingWaitlist>
    {
        public Guid SalonId { get; set; }
        public Guid? PreferredNailArtistId { get; set; }
        public DateTime RequestedDate { get; set; }
        public TimeSpan RequestedStartTime { get; set; }
        public int EstimatedDuration { get; set; }
        public List<WaitlistItemRequestDTO> WaitlistItems { get; set; } = new();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<JoinWaitlistRequestDTO, BookingWaitlist>()
                   .IgnoreAllNonExisting();
        }
    }
}
