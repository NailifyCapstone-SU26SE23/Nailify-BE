using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.QuizRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IRecommendationService
    {
        Task<ApiResult<List<RecommendedNailVariantResponseDTO>>> SubmitQuizAnswersAsync(Guid userId, SubmitQuizAnswersRequestDto request);
        Task<ApiResult<List<RecommendedNailVariantResponseDTO>>> GetRecommendationsAsync(Guid userId, int limit = 10);
        Task<ApiResult<PagedList<RecommendedNailVariantResponseDTO>>> GetRecommendationsFeedAsync(Guid userId, int pageNumber, int pageSize);
    }
}
