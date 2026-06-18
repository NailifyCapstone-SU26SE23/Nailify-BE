using Nailify.Capstone.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Common.Interfaces
{
    public interface IEventEntity
    {
        void AddDomainEvent(BaseEvent domainEvent);
        void RemoveDomainEvent(BaseEvent domainEvent);
        void ClearDomainEvents();
        IReadOnlyCollection<BaseEvent> GetDomainEvents();
    }
}
