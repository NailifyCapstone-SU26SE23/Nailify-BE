using Nailify.Capstone.Domain.Common.Events;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    
    public class Booking : EventEntity
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalonId { get; set; }
        public Guid? NailArtistId { get; set; }
        public Guid? ChairId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public BookingStatus Status { get; set; }
        public decimal? Price { get; set; } 
        public decimal? Discount { get; set; }
        public decimal? TotalPrice { get; set; }
        public int TotalDuration { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CheckInImageUrl { get; set; }
        public string? CheckOutImagesUrl { get; set; }
        public string? QRCode { get; set; }
        public bool IsRated { get; set; } = false;
        public decimal? AmountDue { get; set; }
        public decimal? AmountPaid { get; set; }

        public bool IsRefunded { get; set; } = false;

        // ThanhDT
        public DateTime? ActualCheckInTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public bool IsLateArrival { get; set; }
        // ThanhDT
        // BookingId của lịch hẹn gốc
        public Guid? WarrantyForBookingId { get; set; }
        // Ngày và giờ mới được đề xuất.
        public DateTime? ProposedBookingDate { get; set; }
        public TimeSpan? ProposedStartTime { get; set; }
        public string? ProposedBy { get; set; }
        public string? RescheduleReason { get; set; }
        public virtual Booking? WarrantyForBooking { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual Salon Salon { get; set; } = null!;
        public virtual NailArtist? NailArtist { get; set; }
        public virtual BookingRating? Rating { get; set; }
        public virtual Chair? Chair { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();
        public virtual ICollection<BookingDiscount> BookingDiscounts { get; set; } = new List<BookingDiscount>();

        public void Created(Guid customerId)
        {
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                BookingStatus.Pending,
                BookingStatus.Pending,
                "BookingCreated",
                "Đơn đặt lịch được tạo thành công bởi khách hàng. Mã QR (Base64) đã được khởi tạo.",
                customerId
            ));
        }
        public void CreatedCustom(Guid customerId, string customNailName)
        {
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                BookingStatus.Pending,
                BookingStatus.Pending,
                "CustomBookingCreated",
                $"Khách hàng gửi yêu cầu đặt mẫu nail tùy chỉnh '{customNailName}'. Chờ quản lý phân bổ thợ.",
                customerId
            ));
        }
        public void CheckInFromQr(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.CheckedIn;
            UpdatedAt = DateTime.UtcNow;
            ActualCheckInTime = DateTime.UtcNow.AddHours(7);
            IsLateArrival = DateTime.UtcNow.AddHours(7) > BookingDate.Date.Add(StartTime).AddMinutes(15);
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.CheckedIn,
                "CheckedIn",
                "Xác thực mã QR thành công. Trạng thái đơn hàng chuyển sang CheckedIn.",
                actorId
            ));
        }
        public void CheckIn(string imageUrl, Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.CheckedIn;
            CheckInImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
            ActualCheckInTime = DateTime.UtcNow.AddHours(7);
            IsLateArrival = DateTime.UtcNow.AddHours(7) > BookingDate.Date.Add(StartTime).AddMinutes(15);
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.CheckedIn,
                "CheckedIn",
                $"Check-in thành công. Đã chụp trạng thái tay trước khi làm: {imageUrl}",
                actorId
            ));
        }
        public void CheckInWithoutImage(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.CheckedIn;
            UpdatedAt = DateTime.UtcNow;
            ActualCheckInTime = DateTime.UtcNow.AddHours(7);
            IsLateArrival = DateTime.UtcNow.AddHours(7) > BookingDate.Date.Add(StartTime).AddMinutes(15);
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.CheckedIn,
                "CheckIn",
                "Khách hàng đã check-in.",
                actorId
            ));
        }
        public void CompleteService(string finalUrls, Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.ServiceCompleted;
            CheckOutImagesUrl = finalUrls;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.ServiceCompleted,
                "ServiceCompleted",
                $"Thợ nail đã hoàn thành các dịch vụ. Ảnh trạng thái tay sau khi làm: {finalUrls}",
                actorId
            ));
        }

        public void CheckOut(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Completed,
                "Completed",
                "Khách hàng đã thanh toán hóa đơn và hoàn thành thủ tục check-out.",
                actorId
            ));
        }
        public void Updated(decimal oldPrice, int oldDuration, Guid actorId)
        {
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                Status,
                Status,
                "BookingUpdated",
                $"Đơn đặt lịch được cập nhật. Tổng tiền mới: {Price}. Tổng thời gian: {TotalDuration} phút.",
                actorId
            ));
        }
        public void Cancel(Guid customerId, string reason)
        {
            var oldStatus = Status;
            Status = BookingStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Cancelled,
                "Cancelled",
                $"Hủy đơn từ trạng thái '{oldStatus}' sang 'Cancelled'. Lý do: {reason}",
                customerId
            ));
            AddDomainEvent(new SlotFreedEvent(SalonId, NailArtistId, BookingDate, StartTime, TotalDuration));
        }
        public void Confirm(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Approved,
                "BookingConfirmed",
                "Quản lý Salon xác nhận duyệt đơn đặt lịch.",
                actorId
            ));
        }
        public void Reject(Guid actorId, string reason)
        {
            var oldStatus = Status;
            Status = BookingStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Rejected,
                "BookingRejected",
               $"Quản lý Salon từ chối đơn đặt lịch. Lý do: {reason}",
                actorId
            ));
            AddDomainEvent(new SlotFreedEvent(SalonId, NailArtistId, BookingDate, StartTime, TotalDuration));
        }
        public void StartService(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
            ActualStartTime = DateTime.UtcNow.AddHours(7);
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.InProgress,
                "ServiceStarted",
                "Thợ làm móng bắt đầu thực hiện các dịch vụ trong đơn.",
                actorId
            ));
        }
        public void ReceptionistAssignArtist(Guid artistId, string artistName, Guid actorId)
        {
            NailArtistId = artistId;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                Status, // Trạng thái giữ nguyên
                Status, // Trạng thái giữ nguyên
                "ReceptionistAssignedArtist",
                $"Tiếp tân đã chỉ định thợ {artistName} thực hiện dịch vụ cho đơn hàng.",
                actorId
            ));
        }
        public void AssignChair(Guid chairId, string chairName, Guid actorId)
        {
            ChairId = chairId;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                Status, // Trạng thái giữ nguyên
                Status, // Trạng thái giữ nguyên
                "ChairAssigned",
                $"Đã phân bổ ghế {chairName} cho khách hàng.",
                actorId
            ));
        }

        // Hoàn thành bảo hành
        public void CheckOutWarranty(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.Repaired;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Repaired,
                "Repaired",
                "Đơn bảo hành đã hoàn thành sửa chữa.",
                actorId
            ));
        }
        // Khách yêu cầu đổi lịch
        public void RequestReschedule(DateTime newDate, TimeSpan newTime, string reason, Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.ReschedulePending;
            ProposedBookingDate = newDate;
            ProposedStartTime = newTime;
            ProposedBy = "Customer";
            RescheduleReason = reason;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.ReschedulePending,
                "RescheduleRequested",
                $"Khách hàng yêu cầu đổi lịch sang ngày {newDate:dd/MM/yyyy} lúc {newTime}. Lý do: {reason}",
                actorId
            ));
        }

        // Salon đề xuất một giờ hẹn cho khách hàng
        public void SuggestAlternativeTime(DateTime suggestedDate, TimeSpan suggestedTime, string reason, Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.RescheduleSuggested;
            ProposedBookingDate = suggestedDate;
            ProposedStartTime = suggestedTime;
            ProposedBy = "Manager";
            RescheduleReason = reason;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.RescheduleSuggested,
                "RescheduleSuggested",
                $"Quản lý Salon đề xuất đổi lịch sang ngày {suggestedDate:dd/MM/yyyy} lúc {suggestedTime}. Lý do: {reason}",
                actorId
            ));
        }
        // Acept với đề xuất đôiỉ lịch
        public void AcceptReschedule(Guid actorId)
        {
            if (!ProposedBookingDate.HasValue || !ProposedStartTime.HasValue) return;
            var oldStatus = Status;
            var oldDate = BookingDate;
            var oldTime = StartTime;
            BookingDate = ProposedBookingDate.Value;
            StartTime = ProposedStartTime.Value;
            ProposedBookingDate = null;
            ProposedStartTime = null;
            ProposedBy = null;
            RescheduleReason = null;
            Status = BookingStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Approved,
                "RescheduleAccepted",
                $"Xác nhận thay đổi lịch hẹn từ ngày {oldDate:dd/MM/yyyy} lúc {oldTime} sang ngày {BookingDate:dd/MM/yyyy} lúc {StartTime}.",
                actorId
            ));
            AddDomainEvent(new SlotFreedEvent(SalonId, NailArtistId, oldDate, oldTime, TotalDuration));
        }
        // Từ chối yêu cầu đổi lịch
        public void DeclineReschedule(Guid actorId)
        {
            var oldStatus = Status;
            ProposedBookingDate = null;
            ProposedStartTime = null;
            ProposedBy = null;
            RescheduleReason = null;
            Status = BookingStatus.Approved; // Hoặc trạng thái cũ trước khi đề xuất
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                oldStatus,
                BookingStatus.Approved,
                "RescheduleDeclined",
                "Yêu cầu/Đề xuất đổi lịch bị từ chối. Lịch hẹn giữ nguyên.",
                actorId
            ));
        }
        public void TransferToSalon(Guid newSalonId, Guid? newNailArtistId, Guid actorId, string reason)
        {
            var oldSalonId = SalonId;
            var oldArtistId = NailArtistId;

            SalonId = newSalonId;
            NailArtistId = newNailArtistId;
            ChairId = null; // Reset chair assignment when transferring
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new BookingStatusChangedEvent(
                BookingId,
                Status,
                Status,
                "SalonTransferred",
                $"Booking chuyển từ chi nhánh {oldSalonId} -> {newSalonId}. " + $"Thợ mới: {newNailArtistId?.ToString() ?? "chưa được phân công"}. Lý do: {reason}",
                actorId
            ));

            AddDomainEvent(new SlotFreedEvent(oldSalonId, oldArtistId, BookingDate, StartTime, TotalDuration));
        }
    }
}
