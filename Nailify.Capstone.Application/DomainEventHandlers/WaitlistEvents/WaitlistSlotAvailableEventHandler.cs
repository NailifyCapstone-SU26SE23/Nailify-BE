using MediatR;
using Nailify.Capstone.Application.Common.Models;
using Nailify.Capstone.Domain.Common.Events.WaitlistEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DomainEventHandlers.WaitlistEvents
{
    public class WaitlistSlotAvailableEventHandler : INotificationHandler<DomainEventNotification<WaitlistSlotAvailableEvent>>
    {
        public Task Handle(DomainEventNotification<WaitlistSlotAvailableEvent> notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
