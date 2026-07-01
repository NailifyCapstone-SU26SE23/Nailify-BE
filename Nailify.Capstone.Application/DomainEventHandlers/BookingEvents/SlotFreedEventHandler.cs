using MediatR;
using Nailify.Capstone.Application.Common.Models;
using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DomainEventHandlers.BookingEvents
{
    public class SlotFreedEventHandler : INotificationHandler<DomainEventNotification<SlotFreedEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduledJobService _scheduledJobService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        public SlotFreedEventHandler(IUnitOfWork unitOfWork, IScheduledJobService scheduledJobService, INotificationService notificationService,
          IEmailService emailService,
          IEmailTemplateService emailTemplateService)
        {
            _unitOfWork = unitOfWork;
            _scheduledJobService = scheduledJobService;
            _notificationService = notificationService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }
        public async Task Handle(DomainEventNotification<SlotFreedEvent> notification, CancellationToken cancellationToken)
        {
            var e = notification.DomainEvent;

            var nextEntry = await _unitOfWork.BookingWaitlistRepository.GetNextWaitingEntryAsync(e.SalonId, e.BookingDate, e.StartTime);
            if(nextEntry == null)
            {
                return;
            }
            // Promote client: set status to Notified and grant 15 minutes TTL
            nextEntry.Status = WaitlistStatus.Notified;
            nextEntry.NotifiedAt = DateTime.UtcNow;
            nextEntry.ExpiresAt = DateTime.UtcNow.AddMinutes(15);
            _unitOfWork.BookingWaitlistRepository.Update(nextEntry);
            // 1. Tạo Job ngầm đếm ngược 15 phút trên Hangfire bằng Interface IScheduledJobService
            _scheduledJobService.Schedule<IWaitlistJobExecutor>(
                x => x.CancelIfExpiredAsync(nextEntry.WailistId),
                TimeSpan.FromMinutes(15)
            );
            // 2. Gửi thông báo thời gian thực qua SignalR
            await _notificationService.SendNotificationToUserAsync(
                nextEntry.CustomerId.ToString(),
                "WaitlistPromoted",
                new
                {
                    WaitlistId = nextEntry.WailistId,
                    Message = "Đã có slot trống! Bạn có 15 phút để xác nhận chuyển thành lịch hẹn chính thức."
                }
            );
            var customerUser = await _unitOfWork.UserRepository.GetByIdAsync(nextEntry.CustomerId);
            if (customerUser != null && !string.IsNullOrEmpty(customerUser.Email))
            {
                var confirmUrl = $"https://localhost:7066/api/Waitlists/{nextEntry.WailistId}/confirm-via-email";

                var emailBody = _emailTemplateService.GenerateWaitlistConfirmationEmail(
                    customerUser.FirstName + " " + customerUser.LastName,
                    nextEntry.RequestedStartTime.ToString(@"hh\:mm"),
                    nextEntry.RequestedDate.ToString("dd/MM/yyyy"),
                    confirmUrl
                );
                await _emailService.SendEmailAsync(new MailRequest
                {
                    ToAddress = customerUser.Email,
                    Subject = "Nailify - Thông báo trống lịch hẹn trong hàng chờ!",
                    Body = emailBody
                });
            }
        }
    }
}
