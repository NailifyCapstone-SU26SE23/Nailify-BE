using System.Collections.Generic;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalActiveSalons { get; set; }
        public decimal TotalPlatformRevenue { get; set; }
        public int TotalRegisteredCustomers { get; set; }
        public int TotalActiveStaff { get; set; }
        public double PlatformAverageRating { get; set; }
        public int ActivePromotionsRunning { get; set; }

        public ChartResponse<decimal> RevenueTrend { get; set; } = new();
        public ChartResponse<int> UserGrowth { get; set; } = new();
    }
}
