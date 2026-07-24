using System;

namespace Nailify.Capstone.Domain.Common.Events.BookingEvents
{
    public class SlotFreedEvent : BaseEvent
    {
        public Guid SalonId { get; }
        public Guid? NailArtistId { get; }
        public DateTime BookingDate { get; }
        public TimeSpan StartTime { get; }
        public int Duration { get; }

        public SlotFreedEvent(Guid salonId, Guid? nailArtistId, DateTime bookingDate, TimeSpan startTime, int duration)
        {
            SalonId = salonId;
            NailArtistId = nailArtistId;
            BookingDate = bookingDate;
            StartTime = startTime;
            Duration = duration;
        }
        public SlotFreedEvent(Guid salonId, DateTime bookingDate, TimeSpan startTime)
        {
            SalonId = salonId;
            BookingDate = bookingDate.Date;
            StartTime = startTime;
        }
    }
}
