using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services.BackgroundJobs
{
    public class WaitlistJobExecutor : IWaitlistJobExecutor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        public WaitlistJobExecutor(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }
        public async Task CancelIfExpiredAsync(Guid waitlistId)
        {
            var waitlist = await _unitOfWork.BookingWaitlistRepository.GetByIdAsync(waitlistId);

            // Chỉ xử lý nếu khách ở trạng thái Notified và đã quá thời gian hết hạn
            if (waitlist != null && waitlist.Status == WaitlistStatus.Notified)
            {
                waitlist.Status = WaitlistStatus.Expired;

                // Kích hoạt SlotFreedEvent để hệ thống tự động đẩy người tiếp theo lên
                var freedEvent = new SlotFreedEvent(waitlist.SalonId, waitlist.RequestedDate, waitlist.RequestedStartTime);
                waitlist.AddDomainEvent(freedEvent);
                _unitOfWork.BookingWaitlistRepository.Update(waitlist);
                await _unitOfWork.SaveChangesAsync();
                // Gửi thông báo SignalR cho khách hàng báo lịch hẹn đã hết hạn xác nhận
                await _notificationService.SendNotificationToUserAsync(
                    waitlist.CustomerId.ToString(),
                    "WaitlistExpired",
                    new { Message = "Thời gian xác nhận lịch hẹn từ hàng chờ (15 phút) đã hết hạn." }
                );
            }
        }
        public async Task ClearDailyWaitlistAsync()
        {
            var today = DateTime.UtcNow.AddHours(7).Date;

            // Tìm tất cả các waitlist còn tồn đọng của ngày hôm nay hoặc trước đó mà chưa hoàn thành
            var pendingWaitlists = await _unitOfWork.BookingWaitlistRepository.GetExpiredOrPastEntriesAsync(today);

            foreach (var entry in pendingWaitlists)
            {
                if (entry.Status == WaitlistStatus.Waiting || entry.Status == WaitlistStatus.Notified)
                {
                    entry.Status = WaitlistStatus.Cancelled;
                    _unitOfWork.BookingWaitlistRepository.Update(entry);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
