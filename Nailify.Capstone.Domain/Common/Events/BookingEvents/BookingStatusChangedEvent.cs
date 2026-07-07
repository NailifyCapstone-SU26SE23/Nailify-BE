using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Common.Events.BookingEvents
{
    public class BookingStatusChangedEvent : BaseEvent
    {
        public Guid BookingId { get; }
        public BookingStatus OldStatus { get; }
        public BookingStatus NewStatus { get; }
        public string EventType { get; }
        public string Payload { get; }
        public Guid? ActorId { get; }
        public BookingStatusChangedEvent(Guid bookingId, BookingStatus oldStatus, BookingStatus newStatus, string eventType, string payload, Guid? actorId = null)
        {
            BookingId = bookingId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            EventType = eventType;
            Payload = payload;
            ActorId = actorId == Guid.Empty ? null : actorId;
        }
    }
}
