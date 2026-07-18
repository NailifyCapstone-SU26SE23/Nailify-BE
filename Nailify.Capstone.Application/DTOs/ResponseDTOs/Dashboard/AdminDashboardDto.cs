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
        public ChartResponse<decimal> TopPerformingSalons { get; set; } = new();
        public ChartResponse<int> UserGrowth { get; set; } = new();
        public ChartResponse<int> GlobalServicePopularity { get; set; } = new();
        public List<SalonRatingDistributionDto> SalonRatingDistribution { get; set; } = new();
        public List<PromotionPerformanceDto> GlobalPromotionPerformance { get; set; } = new();
    }

    public class SalonRatingDistributionDto
    {
        public Guid SalonId { get; set; }
        public string SalonName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }

    public class PromotionPerformanceDto
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public decimal DiscountGiven { get; set; }
        public decimal RevenueGenerated { get; set; }
    }

}
