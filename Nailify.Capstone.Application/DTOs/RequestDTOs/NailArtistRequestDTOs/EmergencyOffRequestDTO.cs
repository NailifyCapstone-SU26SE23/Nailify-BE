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

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs
{
    public class EmergencyOffRequestDTO
    {
        public DateTime OffDate { get; set; }
        public string Reason { get; set; } = "Sự cố sức khỏe/nghỉ đột xuất";
    }
    public class EmergencyOffResultDTO
    {
        public Guid NailArtistId { get; set; }
        public DateTime OffDate { get; set; }
        // Tổng số đơn hẹn trong ngày bị ảnh hưởng
        public int TotalAffectedBookings { get; set; }
        // Số đơn đã tự động chuyển sang thợ khác giữ nguyên giờ
        public int AutoReassignedCount { get; set; }
        // Số đơn đã tự động tìm slot dời giờ(+/- 30-60p)
        public int RescheduleSuggestedCount { get; set; }
        // Số đơn phải tự động Hủy & Hoàn 100% cọc
        public int CancelledAndRefundedCount { get; set; }
        // Danh sách chi tiết xử lý cho từng đơn
        public List<EmergencyBookingHandlingDetailDTO> ProcessingDetails { get; set; } = new();
    }

    public class EmergencyBookingHandlingDetailDTO : IMapFrom<Booking>
    {
        public Guid BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        // Gio hen ban dau cua khach
        public TimeSpan OriginalStartTime { get; set; }
        public EmergencyHandlingResult HandlingResult { get; set; }
        // ID tho moi duoc he thong phan cong
        public Guid? NewAssignedArtistId { get; set; }
        public string? NewAssignedArtistName { get; set; }
        // Khung gio moi duoc he thong tim va de xuat neu khong co tho nao ranh vao slot do
        public TimeSpan? SuggestedStartTime { get; set; }
        public string? VoucherCode { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Booking, EmergencyBookingHandlingDetailDTO>()
                .ForMember(d => d.OriginalStartTime, opt => opt.MapFrom(s => s.StartTime))
                .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer != null && s.Customer.User != null ? s.Customer.User.FirstName + " " + s.Customer.User.LastName : (s.Customer != null ? s.Customer.User.FirstName + " " + s.Customer.User.LastName : string.Empty)))
                .IgnoreAllNonExisting();
        }
    }
}
