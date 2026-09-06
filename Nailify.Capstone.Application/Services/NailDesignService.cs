using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System.Text.Json;

namespace Nailify.Capstone.Application.Services
{
    public class NailDesignService : INailDesignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public NailDesignService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResult<PagedList<NailDesignDto>>> GetPagedNailDesignsAsync(
            int pageNumber,
            int pageSize,
            string? name = null,
            IEnumerable<int>? categoryIds = null,
            Guid? userId = null)
        {
            var pagedResult = await _unitOfWork.NailDesignRepository.GetPagedActiveNailDesignsAsync(
                pageNumber,
                pageSize,
                name,
                categoryIds);
            var mappedItems = _mapper.Map<List<NailDesignDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<NailDesignDto>(
                mappedItems,
                pagedResult.MetaData.TotalItems,
                pageNumber,
                pageSize
            );
            await PopulateFavoriteStatusAsync(mappedItems, userId);

            return new ApiSuccessResult<PagedList<NailDesignDto>>(resultPagedList, "Lấy danh sách mẫu nail thành công.");
        }

        public async Task<ApiResult<NailDesignDto>> GetNailDesignByIdAsync(int id, Guid? userId = null)
        {
            var design = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(id);
            if (design == null || design.Status == "InActive")
            {
                return new ApiErrorResult<NailDesignDto>("Không tìm thấy mẫu nail.");
            }

            var designDto = _mapper.Map<NailDesignDto>(design);
            await PopulateFavoriteStatusAsync(new[] { designDto }, userId);
            return new ApiSuccessResult<NailDesignDto>(designDto, "Lấy thông tin mẫu nail thành công.");
        }

        public async Task<ApiResult<NailSummaryDto>> GetNailDesignSummaryAsync(int id)
        {
            var cacheKey = $"NailDesignSummary_{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedSummary = JsonSerializer.Deserialize<NailSummaryDto>(cachedData);
                if (cachedSummary != null)
                {
                    return new ApiSuccessResult<NailSummaryDto>(cachedSummary, "Lấy tổng quan mẫu nail thành công.");
                }
            }

            var summary = await _unitOfWork.NailDesignRepository.GetNailDesignSummaryAsync(id);
            if (summary == null)
            {
                return new ApiErrorResult<NailSummaryDto>("Không tìm thấy mẫu nail.");
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(summary), cacheOptions);

            return new ApiSuccessResult<NailSummaryDto>(summary, "Lấy tổng quan mẫu nail thành công.");
        }

        public async Task<ApiResult<NailDesignDto>> CreateNailDesignAsync(NailDesignCreateRequest request, string? imageUrl = null)
        {
            var invalidCategoryIds = await GetInvalidCategoryIdsAsync(request.CategoryIds);
            if (invalidCategoryIds.Any())
            {
                return new ApiErrorResult<NailDesignDto>($"Không tìm thấy danh mục: {string.Join(", ", invalidCategoryIds)}.");
            }

            var design = _mapper.Map<NailDesign>(request);
            design.Status = "Active";
            design.ImageUrl = imageUrl ?? string.Empty;
            design.NailCategories = request.CategoryIds
                .Distinct()
                .Select(categoryId => new NailCategory { CategoryId = categoryId })
                .ToList();

            await _unitOfWork.NailDesignRepository.CreateAsync(design);
            await _unitOfWork.SaveChangesAsync();

            var createdDesign = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(design.NailDesignId);
            var designDto = _mapper.Map<NailDesignDto>(createdDesign);

            return new ApiSuccessResult<NailDesignDto>(designDto, "Tạo mẫu nail thành công.");
        }

        public async Task<ApiResult<NailDesignDto>> UpdateNailDesignAsync(int id, NailDesignUpdateRequest request, string? newImageUrl = null)
        {
            var existingDesign = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(id);
            if (existingDesign == null || existingDesign.Status == "InActive")
            {
                return new ApiErrorResult<NailDesignDto>("Không tìm thấy mẫu nail.");
            }

            var invalidCategoryIds = await GetInvalidCategoryIdsAsync(request.CategoryIds);
            if (invalidCategoryIds.Any())
            {
                return new ApiErrorResult<NailDesignDto>($"Không tìm thấy danh mục: {string.Join(", ", invalidCategoryIds)}.");
            }

            _mapper.Map(request, existingDesign);
            existingDesign.NailCategories.Clear();

            foreach (var categoryId in request.CategoryIds.Distinct())
            {
                existingDesign.NailCategories.Add(new NailCategory
                {
                    NailDesignId = existingDesign.NailDesignId,
                    CategoryId = categoryId
                });
            }

            existingDesign.ImageUrl = !string.IsNullOrWhiteSpace(newImageUrl)
                ? newImageUrl
                : request.ExistingImageUrl ?? existingDesign.ImageUrl;

            _unitOfWork.NailDesignRepository.Update(existingDesign);
            await _unitOfWork.SaveChangesAsync();

            var updatedDesign = await _unitOfWork.NailDesignRepository.GetNailDesignWithCategoriesAsync(id);
            var designDto = _mapper.Map<NailDesignDto>(updatedDesign);

            return new ApiSuccessResult<NailDesignDto>(designDto, "Cập nhật mẫu nail thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailDesignAsync(int id)
        {
            var design = await _unitOfWork.NailDesignRepository.GetByIdAsync(id);
            if (design == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy mẫu nail.");
            }

            _unitOfWork.NailDesignRepository.Delete(design);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa mẫu nail thành công.");
        }

        public async Task<ApiResult<List<NailDesignDto>>> GetNailDesignsByCategoryAsync(int categoryId, Guid? userId = null)
        {
            var designs = await _unitOfWork.NailDesignRepository.GetNailDesignsByCategoryAsync(categoryId);
            var designDtos = _mapper.Map<List<NailDesignDto>>(designs);
            await PopulateFavoriteStatusAsync(designDtos, userId);

            return new ApiSuccessResult<List<NailDesignDto>>(designDtos, "Lấy danh sách mẫu nail theo danh mục thành công.");
        }

        private async Task PopulateFavoriteStatusAsync(IEnumerable<NailDesignDto> designs, Guid? userId)
        {
            if (userId == null)
            {
                return;
            }

            var designList = designs.ToList();
            var designIds = designList.Select(design => design.NailDesignId).ToHashSet();
            var variantIds = designList
                .SelectMany(design => design.NailVariants)
                .Select(variant => variant.NailVariantId)
                .ToHashSet();
            if (!designIds.Any() && !variantIds.Any())
            {
                return;
            }

            var favorites = await _unitOfWork.FavoriteNailRepository
                .GetFavoritesByDesignAndVariantIdsAsync(userId.Value, designIds, variantIds);

            var favoriteByDesignId = favorites
                .Where(favorite => favorite.NailVariantId == null && favorite.NailDesignId != null)
                .GroupBy(favorite => favorite.NailDesignId!.Value)
                .ToDictionary(group => group.Key, group => group.First());
            var favoriteByVariantId = favorites
                .Where(favorite => favorite.NailVariantId != null)
                .GroupBy(favorite => favorite.NailVariantId!.Value)
                .ToDictionary(group => group.Key, group => group.First());

            foreach (var design in designList)
            {
                if (favoriteByDesignId.TryGetValue(design.NailDesignId, out var favorite))
                {
                    design.IsFavorited = true;
                    design.FavoriteNailId = favorite.FavoriteNailId;
                }

                foreach (var variant in design.NailVariants)
                {
                    if (favoriteByVariantId.TryGetValue(variant.NailVariantId, out var variantFavorite))
                    {
                        variant.IsFavorited = true;
                        variant.FavoriteNailId = variantFavorite.FavoriteNailId;
                    }
                }
            }
        }

        private async Task<List<int>> GetInvalidCategoryIdsAsync(IEnumerable<int> categoryIds)
        {
            var invalidCategoryIds = new List<int>();

            foreach (var categoryId in categoryIds.Distinct())
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category == null || category.Status == "InActive")
                {
                    invalidCategoryIds.Add(categoryId);
                }
            }

            return invalidCategoryIds;
        }
    }
}
