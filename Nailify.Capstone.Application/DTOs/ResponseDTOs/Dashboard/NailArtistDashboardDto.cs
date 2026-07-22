using System;
using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard
{
    public class NailArtistDashboardDto
    {
        // KPIs
        public int RemainingAppointmentsCount { get; set; }
        public int CompletedAppointmentsCount { get; set; }
        public decimal EstimatedEarnings { get; set; }
        public double AverageRatingScore { get; set; }

        // Widgets & Charts
        public NextCustomerProfileDto? NextCustomer { get; set; }
        public List<ArtistScheduleItemDto> MySchedule { get; set; } = new();
        public ChartResponse<decimal> EarningsTracker { get; set; } = new();
        public List<FeedbackCardDto> RecentFeedback { get; set; } = new();
        public List<string> SkillOverview { get; set; } = new();
        public ChartResponse<double> ServiceTimeEfficiency { get; set; } = new();
    }

    public class NextCustomerProfileDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string PreferredComplexity { get; set; } = string.Empty;
        public int? PreferredNailShapeId { get; set; }
        public string Note { get; set; } = string.Empty;
    }

    public class ArtistScheduleItemDto
    {
        public TimeSpan StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Booking" or "Break"
    }

    public class FeedbackCardDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
