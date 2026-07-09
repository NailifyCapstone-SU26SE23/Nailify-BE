using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class NailVariantRecommendationDto
    {
        public int NailVariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NailDesignId { get; set; }
        public string NailDesignName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public float PredictedScore { get; set; }  // score từ ML model
        public bool IsFallback { get; set; }       // true = popular (cold start)
    }
}
