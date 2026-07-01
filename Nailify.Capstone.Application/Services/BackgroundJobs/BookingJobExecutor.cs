using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services.BackgroundJobs
{
    public class BookingJobExecutor : IBookingJobExecutor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;


        public BookingJobExecutor(IUnitOfWork unitOfWork, INotificationService notificationService, IEmailService emailService, IEmailTemplateService emailTemplateService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }
        public async Task CancelLateBookingsAsync()
        {
            var nowUtc = DateTime.UtcNow;

            // Lấy danh sách booking trong ngày hôm nay ở trạng thái Approved (chưa Check-in)
            // và đã trễ quá 15 phút so với giờ hẹn (BookingDate + StartTime < nowUtc - 15 phút)
            var lateBookings = await _unitOfWork.BookingRepository.GetOverdueApprovedBookingsAsync(nowUtc.Date, nowUtc.TimeOfDay.Add(TimeSpan.FromMinutes(-15)));
            foreach (var booking in lateBookings)
            {
                // Thực hiện hủy lịch của domain (hàm Cancel tự động kích hoạt SlotFreedEvent)
                booking.Cancel(Guid.Empty, "Hệ thống tự động hủy do khách trễ quá 15 phút mà không check-in.");
                _unitOfWork.BookingRepository.Update(booking);

                // Gửi thông báo SignalR cho khách hàng báo hủy lịch do đến muộn
                await _notificationService.SendNotificationToUserAsync(
                    booking.CustomerId.ToString(),
                    "BookingAutoCancelled",
                    new { BookingId = booking.BookingId, Message = "Lịch hẹn của bạn đã tự động hủy do trễ quá 15 phút." }
                );
            }
            if (lateBookings.Any())
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SendBookingReminderEmailAsync(Guid bookingId)
        {
            // Tìm Booking từ DB
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId); // Dùng đồng bộ

            if (booking != null && booking.Status == Domain.Enums.BookingStatus.Approved)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(booking.CustomerId);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var emailBody = _emailTemplateService.GenerateBookingReminderEmail(
                        user.FirstName + " " + user.LastName,
                        booking.Salon?.Name ?? "Nailify Salon",
                        booking.StartTime.ToString(@"hh\:mm"),
                        booking.BookingDate.ToString("dd/MM/yyyy")
                    );
                    var mailRequest = new MailRequest
                    {
                        ToAddress = user.Email,
                        Subject = "Nailify - Nhắc nhở lịch hẹn sắp diễn ra",
                        Body = emailBody
                    };
                    // Dùng SendEmailAsync bất đồng bộ
                    await _emailService.SendEmailAsync(mailRequest);
                }
            }
        }
    }
}
