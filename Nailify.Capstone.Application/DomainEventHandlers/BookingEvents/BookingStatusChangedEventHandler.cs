using MediatR;
using Nailify.Capstone.Application.Common.Models;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DomainEventHandlers.BookingEvents
{
    public class BookingStatusChangedEventHandler : INotificationHandler<DomainEventNotification<BookingStatusChangedEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduledJobService _scheduledJobService;
        public BookingStatusChangedEventHandler(IUnitOfWork unitOfWork, IScheduledJobService scheduledJobService)
        {
            _unitOfWork = unitOfWork;
            _scheduledJobService = scheduledJobService;
        }
        public async Task Handle(DomainEventNotification<BookingStatusChangedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var history = new BookingHistory
            {
                BookingId = domainEvent.BookingId,
                EventType = domainEvent.EventType,
                Payload = domainEvent.Payload,
                ActorId = domainEvent.ActorId == Guid.Empty ? null : domainEvent.ActorId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.BookingHistoryRepository.CreateAsync(history);

            if (domainEvent.NewStatus == BookingStatus.Completed &&
                domainEvent.OldStatus != BookingStatus.Completed &&
                !await _unitOfWork.LoyaltyTransactionRepository.ExistsAsync(t => t.BookingId == domainEvent.BookingId))
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(domainEvent.BookingId);
                var customer = booking == null
                    ? null
                    : await _unitOfWork.CustomerRepository.GetByIdAsync(booking.CustomerId);

                if (booking != null && customer != null)
                {
                    const int earnedPoints = 10;
                    customer.LoyaltyPoint += earnedPoints;
                    customer.LifetimePoints += earnedPoints;

                    var matchedTier = _unitOfWork.LoyaltyTierRepository.FindAll()
                        .Where(t =>
                            (!t.MinLifetimePoints.HasValue || customer.LifetimePoints >= t.MinLifetimePoints.Value) &&
                            (!t.MaxLifetimePoints.HasValue || customer.LifetimePoints <= t.MaxLifetimePoints.Value))
                        .OrderByDescending(t => t.MinLifetimePoints ?? 0)
                        .FirstOrDefault();

                    matchedTier ??= _unitOfWork.LoyaltyTierRepository.FindAll()
                        .FirstOrDefault(t => t.SortOrder == 1);

                    if (matchedTier != null)
                    {
                        customer.LoyaltyTierId = matchedTier.LoyaltyTierId;
                    }

                    _unitOfWork.CustomerRepository.Update(customer);

                    await _unitOfWork.LoyaltyTransactionRepository.CreateAsync(new LoyaltyTransaction
                    {
                        CustomerId = booking.CustomerId,
                        BookingId = booking.BookingId,
                        Points = earnedPoints,
                        TransactionType = LoyaltyTransactionType.Earned,
                        LoyaltyTierIdAtTime = matchedTier?.LoyaltyTierId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            if (domainEvent.NewStatus == BookingStatus.Approved)
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(domainEvent.BookingId);
                if (booking != null)
                {
                    var appointmentTime = booking.BookingDate.Date.Add(booking.StartTime);

                    // Thời điểm gửi mail = Lịch hẹn - 15 phút
                    var reminderTime = appointmentTime.AddMinutes(-15);

                    // Độ trễ từ thời điểm hiện tại (sử dụng UtcNow đồng bộ với database)
                    var delay = reminderTime - DateTime.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        // Trường hợp > 15 phút: Hẹn giờ đúng thời điểm (Schedule)
                        _scheduledJobService.Schedule<IBookingJobExecutor>(
                            x => x.SendBookingReminderEmailAsync(booking.BookingId),
                            delay
                        );
                    }
                    else
                    {
                        // Trường hợp <= 15 phút hoặc sát giờ: Gửi ngay lập tức (Enqueue)
                        _scheduledJobService.Enqueue<IBookingJobExecutor>(
                            x => x.SendBookingReminderEmailAsync(booking.BookingId)
                        );
                    }
                }
            }
        }
    }
}
