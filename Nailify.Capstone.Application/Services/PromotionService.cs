using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.PromotionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<PromotionDto>>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            PromotionType? type = null,
            PromotionScope? scope = null,
            DiscountType? discountType = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var pagedPromotions = await _unitOfWork.PromotionRepository.GetPagedPromotionsAsync(
                pageNumber,
                pageSize,
                type,
                scope,
                discountType,
                startDate,
                endDate);

            var result = new PagedList<PromotionDto>(
                _mapper.Map<List<PromotionDto>>(pagedPromotions.Items),
                pagedPromotions.MetaData.TotalItems,
                pageNumber,
                pageSize);

            return new ApiSuccessResult<PagedList<PromotionDto>>(result, "Lấy danh sách khuyến mãi thành công.");
        }

        public async Task<ApiResult<PagedList<PromotionDto>>> GetTodayPagedAsync(
            int pageNumber,
            int pageSize,
            PromotionType? type = null,
            Guid? customerId = null)
        {
            var promotions = await _unitOfWork.PromotionRepository.GetActivePromotionsForDisplayAsync(DateTime.UtcNow, type);

            if (customerId.HasValue)
            {
                var eligiblePromotions = new List<Promotion>();

                foreach (var promotion in promotions)
                {
                    if (promotion.Scope == PromotionScope.FirstTimeUser &&
                        await HasUserCompletedBookingAsync(customerId.Value))
                    {
                        continue;
                    }

                    if (promotion.UserLimit.HasValue)
                    {
                        var usage = await _unitOfWork.UserPromotionUsageRepository.GetByUserAndPromotionAsync(
                            customerId.Value,
                            promotion.PromotionId);

                        if (usage != null && usage.UsageCount >= promotion.UserLimit.Value)
                        {
                            continue;
                        }
                    }

                    eligiblePromotions.Add(promotion);
                }

                promotions = eligiblePromotions;
            }

            var pagedItems = promotions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedList<PromotionDto>(
                _mapper.Map<List<PromotionDto>>(pagedItems),
                promotions.Count,
                pageNumber,
                pageSize);

            return new ApiSuccessResult<PagedList<PromotionDto>>(result, "Lấy danh sách khuyến mãi thành công.");
        }

        public async Task<ApiResult<PromotionDto>> GetByIdAsync(int id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            return promotion == null
                ? new ApiErrorResult<PromotionDto>("Không tìm thấy khuyến mãi.")
                : new ApiSuccessResult<PromotionDto>(_mapper.Map<PromotionDto>(promotion), "Lấy khuyến mãi thành công.");
        }

        public async Task<ApiResult<List<PromotionDto>>> GetByCategoryIdAsync(int categoryId)
        {
            var promotions = await _unitOfWork.PromotionRepository.GetByCategoryIdAsync(categoryId);
            return new ApiSuccessResult<List<PromotionDto>>(
                _mapper.Map<List<PromotionDto>>(promotions),
                "Lấy danh sách khuyến mãi thành công.");
        }

        public async Task<ApiResult<List<PromotionDto>>> GetByCategoryTypeIdAsync(int categoryTypeId)
        {
            var promotions = await _unitOfWork.PromotionRepository.GetByCategoryTypeIdAsync(categoryTypeId);
            return new ApiSuccessResult<List<PromotionDto>>(
                _mapper.Map<List<PromotionDto>>(promotions),
                "Lấy danh sách khuyến mãi thành công.");
        }

        public async Task<ApiResult<List<PromotionDto>>> GetByNailDesignIdAsync(int nailDesignId)
        {
            var promotions = await _unitOfWork.PromotionRepository.GetByNailDesignIdAsync(nailDesignId);
            return new ApiSuccessResult<List<PromotionDto>>(
                _mapper.Map<List<PromotionDto>>(promotions),
                "Lấy danh sách khuyến mãi thành công.");
        }

        public async Task<ApiResult<PromotionDto>> CreateAsync(PromotionRequest request, string? imageUrl = null)
        {
            var validationError = await ValidateAsync(request);
            
            if (validationError != null)
            {
                return new ApiErrorResult<PromotionDto>(validationError);
            }

            var promotion = _mapper.Map<Promotion>(request);
            NormalizePromotionLimits(promotion);
            promotion.ImageUrl = imageUrl ?? string.Empty;

            if (request.Type == PromotionType.Voucher)
            {
                promotion.IsSelectable = true;
            } else {
                promotion.IsSelectable = false;
            }

            await _unitOfWork.PromotionRepository.CreateAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<PromotionDto>(_mapper.Map<PromotionDto>(promotion), "Tạo khuyến mãi thành công");
        }

        public async Task<ApiResult<PromotionDto>> UpdateAsync(int id, PromotionRequest request, string? imageUrl = null)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                return new ApiErrorResult<PromotionDto>("Không tìm thấy khuyến mãi.");
            }

            var validationError = await ValidateAsync(request, id);
            if (validationError != null)
            {
                return new ApiErrorResult<PromotionDto>(validationError);
            }

            _mapper.Map(request, promotion);
            NormalizePromotionLimits(promotion);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                promotion.ImageUrl = imageUrl;
            }

            if (request.Type == PromotionType.Voucher)
            {
                promotion.IsSelectable = true;
            }
            else
            {
                promotion.IsSelectable = false;
            }

            _unitOfWork.PromotionRepository.Update(promotion);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<PromotionDto>(_mapper.Map<PromotionDto>(promotion), "Cập nhật khuyến mãi thành công.");
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy khuyến mãi.");
            }

            _unitOfWork.PromotionRepository.Delete(promotion);
            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true, "Xóa khuyến mãi thành công.");
        }

        public async Task<List<Promotion>> GetApplicablePromotionsAsync(
    Guid customerId,
    ICollection<BookingItem> items,
    IEnumerable<int>? selectedPromotionIds = null)
        {
            var promotions = await _unitOfWork.PromotionRepository.GetActivePromotionsAsync(DateTime.UtcNow, selectedPromotionIds);
            var hasBookedBefore = await HasUserBookedBeforeAsync(customerId);
            var applicable = new List<Promotion>();

            foreach (var promotion in promotions.Where(promotion => promotion.IsValid()))
            {
                if (promotion.IsSelectable)
                {
                    if (selectedPromotionIds == null || !selectedPromotionIds.Contains(promotion.PromotionId))
                    {
                        continue; 
                    }
                }

                if (promotion.Scope == PromotionScope.FirstTimeUser && hasBookedBefore)
                {
                    continue;
                }

                if (promotion.UserLimit.HasValue)
                {
                    var usage = await _unitOfWork.UserPromotionUsageRepository.GetByUserAndPromotionAsync(customerId, promotion.PromotionId);
                    if (usage != null && usage.UsageCount >= promotion.UserLimit.Value)
                    {
                        continue;
                    }
                }

                foreach (var item in items)
                {
                    if (await IsItemEligibleForPromotionAsync(item, promotion))
                    {
                        applicable.Add(promotion);
                        break;
                    }
                }
            }

            return applicable;
        }

        public async Task<(decimal totalDiscount, List<BookingDiscount> appliedDiscounts)> CalculateDiscountsAsync(
            Booking booking,
            List<Promotion> applicablePromotions)
        {
            decimal totalDiscount = 0;
            var appliedDiscounts = new List<BookingDiscount>();
            var usedPromotions = new HashSet<int>();

            foreach (var item in booking.BookingItems)
            {
                var selectedPromotions = await SelectApplicablePromotionsAsync(item, applicablePromotions, usedPromotions);
                if (!selectedPromotions.Any())
                {
                    continue;
                }

                var lineAmount = GetLineAmount(item);
                decimal itemDiscountTotal = 0;

                foreach (var selectedPromotion in selectedPromotions)
                {
                    var remainingAmount = lineAmount - itemDiscountTotal;
                    if (remainingAmount <= 0)
                    {
                        break;
                    }

                    var discountAmount = Math.Min(CalculateItemDiscount(item, selectedPromotion), remainingAmount);
                    if (discountAmount <= 0)
                    {
                        continue;
                    }

                    usedPromotions.Add(selectedPromotion.PromotionId);
                    totalDiscount += discountAmount;
                    itemDiscountTotal += discountAmount;

                    appliedDiscounts.Add(new BookingDiscount
                    {
                        BookingId = booking.BookingId,
                        Name = selectedPromotion.Name,
                        DiscountAmount = discountAmount,
                        IsAutoApplied = !selectedPromotion.IsSelectable,
                        AppliedDate = DateTime.UtcNow,
                        PromotionId = selectedPromotion.PromotionId
                    });
                }
            }

            return (totalDiscount, appliedDiscounts);
        }

        public async Task UpdateUsageAsync(Guid userId, IEnumerable<BookingDiscount> appliedDiscounts)
        {
            var promotionIds = appliedDiscounts
                .Where(discount => discount.PromotionId.HasValue)
                .Select(discount => discount.PromotionId!.Value)
                .Distinct()
                .ToList();

            foreach (var promotionId in promotionIds)
            {
                var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(promotionId);
                if (promotion == null)
                {
                    continue;
                }

                promotion.CurrentUsageCount++;
                _unitOfWork.PromotionRepository.Update(promotion);

                var usage = await _unitOfWork.UserPromotionUsageRepository.GetByUserAndPromotionAsync(userId, promotionId);
                if (usage == null)
                {
                    await _unitOfWork.UserPromotionUsageRepository.CreateAsync(new UserPromotionUsage
                    {
                        UserId = userId,
                        PromotionId = promotionId,
                        UsageCount = 1,
                        LastUsedDate = DateTime.UtcNow
                    });
                }
                else
                {
                    usage.UsageCount++;
                    usage.LastUsedDate = DateTime.UtcNow;
                    _unitOfWork.UserPromotionUsageRepository.Update(usage);
                }
            }
        }

        private async Task<string?> ValidateAsync(PromotionRequest request, int? excludedId = null)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return "Tên khuyến mãi không được để trống.";
            if (request.DiscountValue <= 0) return "Giá trị giảm giá phải lớn hơn 0";
            if (request.DiscountType == DiscountType.Percentage && request.DiscountValue > 100) return "Phầm trăm giảm giá không được lớn hơn 100.";
            if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate) return "Ngày kết thúc không được trước ngày bắt đầu.";
            if (request.UsageLimit < 0 || request.UserLimit < 0) return "Giới hạn sử dụng không được lớn hơn 0";

            var promotion = _mapper.Map<Promotion>(request);
            if (!promotion.IsValid()) return "Phạm vi khuyến mãi không khớp với đối tượng áp dụng.";

            var duplicate = await _unitOfWork.PromotionRepository.ExistsAsync(promotionEntity =>
                promotionEntity.PromotionId != excludedId &&
                promotionEntity.Name.ToLower() == request.Name.Trim().ToLower());

            return duplicate ? "Tên khuyến mãi đã tồn tại." : null;
        }

        private static void NormalizePromotionLimits(Promotion promotion)
        {
            if (promotion.Scope != PromotionScope.FirstTimeUser)
            {
                return;
            }

            promotion.UsageLimit = null;
            promotion.UserLimit = 1;
        }

        private Task<bool> HasUserBookedBeforeAsync(Guid customerId)
        {
            var hasBooking = _unitOfWork.BookingRepository
                .FindByCondition(booking => booking.CustomerId == customerId && booking.Status == BookingStatus.Completed)
                .Any();

            return Task.FromResult(hasBooking);
        }

        private Task<bool> HasUserCompletedBookingAsync(Guid customerId)
        {
            return _unitOfWork.BookingRepository.ExistsAsync(booking =>
                booking.CustomerId == customerId &&
                booking.Status == BookingStatus.Completed);
        }

        private async Task<List<Promotion>> SelectApplicablePromotionsAsync(
            BookingItem item,
            IEnumerable<Promotion> promotions,
            ISet<int> usedPromotionIds)
        {
            var variant = await GetVariantAsync(item);
            var design = variant?.NailDesign;
            var selectedPromotions = new List<Promotion>();

            foreach (var scope in GetPriorityScopes())
            {
                selectedPromotions.AddRange(promotions.Where(promotion =>
                    promotion.Scope == scope
                    && promotion.Status == "Active"
                    && !usedPromotionIds.Contains(promotion.PromotionId)
                    && IsPromotionMatched(design, promotion)));
            }

            return selectedPromotions;
        }

        private async Task<bool> IsItemEligibleForPromotionAsync(BookingItem item, Promotion promotion)
        {
            var variant = await GetVariantAsync(item);
            return IsPromotionMatched(variant?.NailDesign, promotion);
        }

        private async Task<NailVariant?> GetVariantAsync(BookingItem item)
        {
            if (item.NailVariant != null)
            {
                return item.NailVariant;
            }

            return item.NailVariantId.HasValue
                ? await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(item.NailVariantId.Value)
                : null;
        }

        private static bool IsPromotionMatched(NailDesign? design, Promotion promotion)
        {
            return promotion.Scope switch
            {
                PromotionScope.NailDesign => design?.NailDesignId == promotion.NailDesignId,
                PromotionScope.Category => design?.NailCategories.Any(nailCategory =>
                    nailCategory.CategoryId == promotion.CategoryId) == true,
                PromotionScope.CategoryType => design?.NailCategories.Any(nailCategory =>
                    nailCategory.Category.CategoryTypeId == promotion.CategoryTypeId) == true,
                PromotionScope.All => true,
                PromotionScope.FirstTimeUser => true,
                _ => false
            };
        }

        private static IReadOnlyList<PromotionScope> GetPriorityScopes()
        {
            return
            [
                PromotionScope.NailDesign,
                PromotionScope.Category,
                PromotionScope.CategoryType,
                PromotionScope.FirstTimeUser,
                PromotionScope.All
            ];
        }

        private static decimal CalculateItemDiscount(BookingItem item, Promotion promotion)
        {
            var lineAmount = GetLineAmount(item);

            return promotion.DiscountType switch
            {
                DiscountType.Percentage => lineAmount * (promotion.DiscountValue / 100),
                DiscountType.FixedAmount => Math.Min(promotion.DiscountValue, lineAmount),
                _ => 0
            };
        }

        private static decimal GetLineAmount(BookingItem item)
        {
            return item.Price * Math.Max(item.Quantity, 1);
        }
    }
}
