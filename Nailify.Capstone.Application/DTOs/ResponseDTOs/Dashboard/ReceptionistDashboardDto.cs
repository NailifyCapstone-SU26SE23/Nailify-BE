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
