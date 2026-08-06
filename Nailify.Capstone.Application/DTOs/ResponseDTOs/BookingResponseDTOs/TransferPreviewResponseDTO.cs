using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Application.Mapping;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs
{
    public class TransferPreviewResponseDTO : IMapFrom<Booking>
    {
        public Guid BookingId { get; set; }
        public Guid OriginalSalonId { get; set; }
        public string OriginalSalonName { get; set; } = string.Empty;
        public Guid TargetSalonId { get; set; }
        public string TargetSalonName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Thợ rảnh + đủ skill tại Salon mới trong khung giờ này.
        /// </summary>
        public List<SuggestedArtistResponseDTO> AvailableArtists { get; set; } = new();
        public bool CanTransfer { get; set; }
        public string? WarningMessage { get; set; } // Cảnh báo nếu không có thợ nhưng vẫn có thể chuyển

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Booking, TransferPreviewResponseDTO>()
                   .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice ?? 0))
                   .IgnoreAllNonExisting();
        }
    }
}
