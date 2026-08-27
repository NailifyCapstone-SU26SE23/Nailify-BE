using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard
{
    public class ReceptionistDashboardDto
    {
        // KPIs
        public int CurrentWalkInQueueSize { get; set; }
        public int CurrentWaitlistSize { get; set; }
        public double AverageWaitTimeMinutes { get; set; }
        public int RemainingAppointmentsToday { get; set; }
        public double EstimatedTimeToClearQueueMins { get; set; }
        public string StaffOnDutyText { get; set; } = string.Empty;

        // Lists & Widgets
        public List<WalkInQueueItemDto> LiveWalkInQueue { get; set; } = new();
        public List<WaitlistDashboardItemDto> LiveWaitlist { get; set; } = new();
        public List<SalonScheduleItemDto> MasterSalonSchedule { get; set; } = new();
        public List<UpcomingArrivalDto> UpcomingArrivals { get; set; } = new();
        public List<ChairStatusDto> LiveChairStatus { get; set; } = new();
        public List<NoShowAlertDto> NoShowLateAlerts { get; set; } = new();
    }

    public class WalkInQueueItemDto
    {
        public string GuestName { get; set; } = string.Empty;
        public string RequestNote { get; set; } = string.Empty;
        public int QueuePosition { get; set; }
        public int EstimatedWait { get; set; }
    }

    public class WaitlistDashboardItemDto
    {
        public Guid WaitlistId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public TimeSpan RequestedStartTime { get; set; }
        public int EstimatedDuration { get; set; }
        public int Position { get; set; }
        public string PreferredArtistName { get; set; } = string.Empty;
    }

    public class SalonScheduleItemDto
    {
        public Guid? BookingId { get; set; }
        public Guid? ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class UpcomingArrivalDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
        public string AssignedArtistName { get; set; } = string.Empty;
    }

    public class ChairStatusDto
    {
        public Guid ChairId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public string CurrentCustomer { get; set; } = string.Empty;
    }

    public class NoShowAlertDto
    {
        public Guid BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int MinutesLate { get; set; }
    }
}
