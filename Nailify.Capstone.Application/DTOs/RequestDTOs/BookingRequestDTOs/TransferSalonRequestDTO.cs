using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs
{
    public class TransferSalonRequestDTO : IMapFrom<Booking>
    {
        public Guid TargetSalonId { get; set; }

        /// <summary>
        /// Thợ tại Salon mới. Có thể null - gán thợ sau bằng ReceptionistAssignArtist
        /// </summary>
        public Guid? NewNailArtistId { get; set; }
        public string Reason { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TransferSalonRequestDTO, Booking>()
                   .IgnoreAllNonExisting();
        }
    }
}
