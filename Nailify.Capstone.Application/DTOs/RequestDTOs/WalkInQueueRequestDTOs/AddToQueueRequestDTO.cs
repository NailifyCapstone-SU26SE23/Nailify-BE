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
    public class AddToQueueRequestDTO : IMapFrom<WalkInQueue>
    {
        public Guid SalonId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OriginalBookingId { get; set; }
        public Guid? AssignedNailArtistId { get; set; } // Thêm trường chọn thợ móng
        public string? GuestName { get; set; }
        public string? GuestPhone { get; set; }
        public string? RequestNote { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<AddToQueueRequestDTO, WalkInQueue>()
                   .IgnoreAllNonExisting();
        }
    }
}
