using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.PromotionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IPromotionService
    {
        Task<ApiResult<PagedList<PromotionDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            PromotionType? type = null,
            PromotionScope? scope = null,
            DiscountType? discountType = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
        Task<ApiResult<PagedList<PromotionDto>>> GetTodayPagedAsync(
            int pageNumber,
            int pageSize,
            PromotionType? type = null,
            Guid? customerId = null);
        Task<ApiResult<PromotionDto>> GetByIdAsync(int id);
        Task<ApiResult<List<PromotionDto>>> GetByCategoryIdAsync(int categoryId);
        Task<ApiResult<List<PromotionDto>>> GetByCategoryTypeIdAsync(int categoryTypeId);
        Task<ApiResult<List<PromotionDto>>> GetByNailDesignIdAsync(int nailDesignId);
        Task<ApiResult<PromotionDto>> CreateAsync(PromotionRequest request, string? imageUrl = null);
        Task<ApiResult<PromotionDto>> UpdateAsync(int id, PromotionRequest request, string? imageUrl = null);
        Task<ApiResult<bool>> DeleteAsync(int id);
        Task<ApiResult<PromotionDto>> AddVoucherForRescheduleAsync(Guid bookingId);
        Task<ApiResult<PromotionDto>> AddVoucherForCancelledAsync(Guid bookingId);
        Task<List<Promotion>> GetApplicablePromotionsAsync(Guid customerId, ICollection<BookingItem> items, IEnumerable<int>? selectedPromotionIds = null);
        Task<(decimal totalDiscount, List<BookingDiscount> appliedDiscounts)> CalculateDiscountsAsync(Booking booking, List<Promotion> applicablePromotions);
        Task UpdateUsageAsync(Guid userId, IEnumerable<BookingDiscount> appliedDiscounts);
    }
}
