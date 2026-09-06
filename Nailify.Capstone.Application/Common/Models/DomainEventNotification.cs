using MediatR;
using Nailify.Capstone.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Common.Models
{
    public class DomainEventNotification<TEvent> : INotification where TEvent : BaseEvent
    {
        public TEvent DomainEvent { get; }
        public DomainEventNotification(TEvent domainEvent) => DomainEvent = domainEvent;
    }
}
