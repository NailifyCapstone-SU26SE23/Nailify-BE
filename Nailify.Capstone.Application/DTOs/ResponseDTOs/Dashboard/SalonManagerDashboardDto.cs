using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard
{
    public class SalonManagerDashboardDto
    {
        // KPIs
        public decimal TodaysRevenue { get; set; }
        public int TotalPendingBookings { get; set; }
        public int TotalCompletedBookings { get; set; }
        public double StaffUtilizationRate { get; set; }
        public decimal AverageTicketValue { get; set; }
        public double CancellationRate { get; set; }

        // Charts
        public ChartResponse<decimal> RevenueBreakdown { get; set; } = new();
        public List<ArtistPerformanceDto> ArtistPerformanceLeaderboard { get; set; } = new();
        public ChartResponse<int> PeakHoursHeatmap { get; set; } = new();
        public List<ChairUtilizationDto> ChairUtilization { get; set; } = new();
        public ChartResponse<double> CustomerRetentionRate { get; set; } = new();
        public List<StaffLeaveAlertDto> StaffLeaveAlerts { get; set; } = new();
    }

    public class ArtistPerformanceDto
    {
        public Guid ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public int CompletedBookings { get; set; }
        public decimal RevenueGenerated { get; set; }
        public double AverageRating { get; set; }
    }

    public class ChairUtilizationDto
    {
        public Guid ChairId { get; set; }
        public string ChairName { get; set; } = string.Empty;
        public List<ChairBookingTimelineDto> Bookings { get; set; } = new();
    }

    public class ChairBookingTimelineDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class StaffLeaveAlertDto
    {
        public string ArtistName { get; set; } = string.Empty;
        public DateTime BreakDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
