using MediatR;
using Nailify.Capstone.Application.Common.Models;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
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
        public BookingStatusChangedEventHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task Handle(DomainEventNotification<BookingStatusChangedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var history = new BookingHistory
            {
                BookingId = domainEvent.BookingId,
                EventType = domainEvent.EventType,
                Payload = domainEvent.Payload,
                ActorId = domainEvent.ActorId,
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
        }
    }
}
