using AutoMapper;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
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
    // SlaViolationAlertDTO là DTO chứa gói dữ liệu cảnh báo khẩn cấp (Payload) để hệ thống tự động bắn qua SignalR tới màn hình POS Manager khi xảy ra sự cố trễ hẹn quá 5 phút (Vi phạm SLA).
    public class SlaViolationAlertDTO : IMapFrom<Booking>
    {
        public Guid SalonId { get; set; }
        public Guid AffectedBookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid CurrentArtistId { get; set; }
        public string CurrentArtistName { get; set; } = string.Empty;
        public int EstimatedDelayMinutes { get; set; }
        public Guid OverrunningBookingOrQueueId { get; set; }
        public List<SuggestedReassignArtistDTO> AvailableAlternativeArtists { get; set; } = new();
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Booking, SlaViolationAlertDTO>()
                .ForMember(dest => dest.AffectedBookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.CurrentArtistId, opt => opt.MapFrom(src => src.NailArtistId ?? Guid.Empty))
                .IgnoreAllNonExisting();
        }
    }
}
