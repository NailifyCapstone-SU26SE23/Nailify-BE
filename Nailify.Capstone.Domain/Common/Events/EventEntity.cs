using Nailify.Capstone.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Common.Events
{
    public abstract class EventEntity : IEventEntity
    {
        private readonly List<BaseEvent> _domainEvents = new();
        public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
        public IReadOnlyCollection<BaseEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    }
}
