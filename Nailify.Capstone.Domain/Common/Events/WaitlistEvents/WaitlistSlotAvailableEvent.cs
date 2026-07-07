using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Common.Events.WaitlistEvents
{
    public class WaitlistSlotAvailableEvent : BaseEvent
    {
        public Guid WaitlistId { get; }
        public Guid CustomerId { get; }
        public DateTime ExpiresAt { get; }
        public WaitlistSlotAvailableEvent(Guid waitlistId, Guid customerId, DateTime expiresAt)
        {
            WaitlistId = waitlistId;
            CustomerId = customerId;
            ExpiresAt = expiresAt;
        }
    }
}
