using MediatR;
using Nailify.Capstone.Application.Common.Models;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Entities;
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
            //await _unitOfWork.SaveChangesAsync();
        }
    }
}
