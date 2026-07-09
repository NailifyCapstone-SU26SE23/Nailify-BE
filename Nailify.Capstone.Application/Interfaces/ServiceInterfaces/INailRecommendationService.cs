using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailRecommendationService
    {
        /// <summary>Train/Retrain model từ DB. Chỉ Admin gọi.</summary>
        Task<ApiResult<ModelTrainingResultDto>> TrainModelAsync();
        /// <summary>Gợi ý top N nail variants cho user. Fallback về Popular nếu cold start.</summary>
        Task<ApiResult<List<NailVariantRecommendationDto>>> GetRecommendationsAsync(Guid userId, int topN = 10);
        /// <summary>Popular designs — fallback khi chưa có model hoặc cold start user.</summary>
        Task<ApiResult<List<NailVariantRecommendationDto>>> GetPopularAsync(int topN = 10);
    }
}
