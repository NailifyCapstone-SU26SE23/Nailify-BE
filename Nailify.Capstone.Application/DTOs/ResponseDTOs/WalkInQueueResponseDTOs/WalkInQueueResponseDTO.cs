using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.WalkInQueueResponseDTOs
{
    public class WalkInQueueResponseDTO : IMapFrom<WalkInQueue>
    {
        public Guid QueueId { get; set; }
        public Guid SalonId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OriginalBookingId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestPhone { get; set; }
        public int QueuePosition { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
        public DateTime? CalledTime { get; set; }
        public DateTime? ServiceStartTime { get; set; }
        public Guid? AssignedNailArtistId { get; set; }
        public string? AssignedNailArtistName { get; set; }
        public string? RequestNote { get; set; }
        public int? EstimatedWait { get; set; }
        public bool IsLateArrival { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<WalkInQueue, WalkInQueueResponseDTO>()
                   .IgnoreAllNonExisting()
                   .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                   .ForMember(x => x.IsLateArrival, opt => opt.MapFrom(src => src.OriginalBookingId.HasValue))
                   .ForMember(x => x.AssignedNailArtistName, opt => opt.MapFrom(src =>
                             src.AssignedNailArtist != null && src.AssignedNailArtist.Account != null
                                ? $"{src.AssignedNailArtist.Account.FirstName} {src.AssignedNailArtist.Account.LastName}"
                                : null));
        }
    }
}
