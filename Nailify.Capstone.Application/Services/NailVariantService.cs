using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System.Text.Json;

namespace Nailify.Capstone.Application.Services
{
    public class NailVariantService : INailVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IRecalculationService _recalculationService;

        public NailVariantService(IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache, IRecalculationService recalculationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _recalculationService = recalculationService;
        }

        public async Task<ApiResult<PagedList<NailVariantDto>>> GetPagedNailVariantsAsync(int pageNumber, int pageSize, int? nailDesignId = null, string? name = null, Guid? userId = null, string? status = null)
        {
            var pagedResult = await _unitOfWork.NailVariantRepository.GetPagedNailVariantsAsync(pageNumber, pageSize, nailDesignId, name, status);
            var mappedItems = _mapper.Map<List<NailVariantDto>>(pagedResult.Items);
            await PopulateFavoriteStatusAsync(mappedItems, userId);
            var resultPagedList = new PagedList<NailVariantDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<NailVariantDto>>(resultPagedList, "Lấy danh sách biến thể thành công");
        }

        public async Task<ApiResult<NailVariantDto>> GetNailVariantByIdAsync(int id, Guid? userId = null)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(id);
            /*
            if (variant == null)
            {
                return new ApiErrorResult<NailVariantDto>("Không tìm thấy biến thể.");
            }
            */
            var variantDto = _mapper.Map<NailVariantDto>(variant);
            await PopulateFavoriteStatusAsync(new[] { variantDto }, userId);
            return new ApiSuccessResult<NailVariantDto>(variantDto, "Lấy thông tin biến thể thành công.");
        }

        public async Task<ApiResult<NailSummaryDto>> GetNailVariantSummaryAsync(int id)
        {
            var cacheKey = $"NailVariantSummary_{id}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedSummary = JsonSerializer.Deserialize<NailSummaryDto>(cachedData);
                if (cachedSummary != null)
                {
                    return new ApiSuccessResult<NailSummaryDto>(cachedSummary, "Lấy tổng quan biến thể thành công.");
                }
            }

            var summary = await _unitOfWork.NailVariantRepository.GetNailVariantSummaryAsync(id);
            if (summary == null)
            {
                return new ApiErrorResult<NailSummaryDto>("Không tìm thấy biến thể.");
            }

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(summary), cacheOptions);

            return new ApiSuccessResult<NailSummaryDto>(summary, "Lấy tổng quan biến thể thành công.");
        }

        public async Task<ApiResult<NailVariantDto>> CreateNailVariantAsync(NailVariantCreateRequest request, string? imageUrl = null)
        {
            var validationError = await ValidateReferencesAsync(request.NailDesignId, request.NailShapeId, request.NailSurfaceId);
            if (validationError != null)
            {
                return new ApiErrorResult<NailVariantDto>(validationError);
            }

            var variant = _mapper.Map<NailVariant>(request);
            variant.ImageUrl = imageUrl ?? string.Empty;
            await _unitOfWork.NailVariantRepository.CreateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            await _recalculationService.RecalculateNailVariantAsync(variant.NailVariantId);

            var createdVariant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(variant.NailVariantId);
            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(createdVariant), "Tạo biến thể thành công.");
        }

        public async Task<ApiResult<NailVariantDto>> UpdateNailVariantAsync(int id, NailVariantUpdateRequest request, string? imageUrl = null)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(id);
            if (variant == null)
            {
                return new ApiErrorResult<NailVariantDto>("Không tìm thấy biến thể.");
            }

            var validationError = await ValidateReferencesAsync(request.NailDesignId, request.NailShapeId, request.NailSurfaceId);
            if (validationError != null)
            {
                return new ApiErrorResult<NailVariantDto>(validationError);
            }

            _mapper.Map(request, variant);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                variant.ImageUrl = imageUrl;
            }

            _unitOfWork.NailVariantRepository.Update(variant);
            await _unitOfWork.SaveChangesAsync();
            await _recalculationService.RecalculateNailVariantAsync(id);

            var updatedVariant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(id);
            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(updatedVariant), "Cập nhật biến thể thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailVariantAsync(int id)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(id);
            if (variant == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy biến thể.");
            }

            _unitOfWork.NailVariantRepository.Delete(variant);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa biến thể móng thành.");
        }

        private async Task<string?> ValidateReferencesAsync(int? nailDesignId, int? nailShapeId, int? nailSurfaceId)
        {
            var design = nailDesignId.HasValue
                ? await _unitOfWork.NailDesignRepository.GetByIdAsync(nailDesignId.Value)
                : null;
            if (design == null)
            {
                return "Không tìm thấy mẫu nail.";
            }

            if (nailShapeId.HasValue && await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId.Value) == null)
            {
                return "Không tìm thấy dáng móng.";
            }

            if (nailSurfaceId.HasValue && await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value) == null)
            {
                return "Không tìm thấy bề mặt móng.";
            }

            return null;
        }

        public async Task<ApiResult<List<NailVariantDto>>> GetCapableNailVariantsAsync(Guid artistId, Guid? userId = null)
        {
            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(artistId);
            if (artist == null)
            {
                return new ApiErrorResult<List<NailVariantDto>>("Không tìm thấy thợ nail.");
            }

            var capableVariants = await _unitOfWork.NailVariantRepository.GetNailVariantsCapableByArtistAsync(artistId);
            var response = _mapper.Map<List<NailVariantDto>>(capableVariants);
            await PopulateFavoriteStatusAsync(response, userId);

            return new ApiSuccessResult<List<NailVariantDto>>(response, "Lấy danh sách mẫu móng thợ có thể thực hiện thành công.");
        }
        private async Task PopulateFavoriteStatusAsync(IEnumerable<NailVariantDto> variants, Guid? userId)
        {
            if (userId == null)
            {
                return;
            }

            var variantList = variants.ToList();
            var variantIds = variantList.Select(variant => variant.NailVariantId).ToHashSet();
            if (!variantIds.Any())
            {
                return;
            }

            var favorites = await _unitOfWork.FavoriteNailRepository
                .GetFavoritesByVariantIdsAsync(userId.Value, variantIds);

            var favoriteByVariantId = favorites
                .GroupBy(favorite => favorite.NailVariantId!.Value)
                .ToDictionary(group => group.Key, group => group.First());

            foreach (var variant in variantList)
            {
                if (favoriteByVariantId.TryGetValue(variant.NailVariantId, out var favorite))
                {
                    variant.IsFavorited = true;
                    variant.FavoriteNailId = favorite.FavoriteNailId;
                }
            }
        }
    }
}
