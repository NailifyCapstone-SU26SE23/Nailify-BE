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
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public decimal TotalPrice { get; set; }
        //public string Status { get; set; }
        public BookingStatus Status { get; set; }
        public string Price { get; set; } = string.Empty;
        public int TotalDuration { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CheckInImageUrl { get; set; }
        public string? CheckOutImagesUrl { get; set; }
        public string? QRCode { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual Salon Salon { get; set; } = null!;
        public virtual NailArtist? NailArtist { get; set; }
        public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
        public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();

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
        }
        public void StartService(Guid actorId)
        {
            var oldStatus = Status;
            Status = BookingStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
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
    }
}
