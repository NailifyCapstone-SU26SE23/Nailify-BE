using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class NailVariantService : INailVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailVariantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<NailVariantDto>>> GetPagedNailVariantsAsync(int pageNumber, int pageSize, int? nailDesignId = null, string? name = null)
        {
            var pagedResult = await _unitOfWork.NailVariantRepository.GetPagedNailVariantsAsync(pageNumber, pageSize, nailDesignId, name);
            var mappedItems = _mapper.Map<List<NailVariantDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<NailVariantDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<NailVariantDto>>(resultPagedList, "Lấy danh sách biến thể thành công");
        }

        public async Task<ApiResult<NailVariantDto>> GetNailVariantByIdAsync(int id)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(id);
            if (variant == null)
            {
                return new ApiErrorResult<NailVariantDto>("Không tìm thấy biến thể mong.");
            }

            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(variant), "Lấy thông tin biến thể mong thành công.");
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
            variant.Price = await CalculateNailVariantPriceAsync(request.NailShapeId, request.NailSurfaceId);
            variant.Duration = await CalculateNailVariantDurationAsync(request.NailShapeId, request.NailSurfaceId);
            await _unitOfWork.NailVariantRepository.CreateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailDesignPriceRangeAsync(variant.NailDesignId);

            var createdVariant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(variant.NailVariantId);
            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(createdVariant), "Tạo biến thể móng thành công.");
        }

        public async Task<ApiResult<NailVariantDto>> UpdateNailVariantAsync(int id, NailVariantUpdateRequest request)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(id);
            if (variant == null)
            {
                return new ApiErrorResult<NailVariantDto>("Không tìm thấy biến thể móng.");
            }

            var previousNailDesignId = variant.NailDesignId;
            var validationError = await ValidateReferencesAsync(request.NailDesignId, request.NailShapeId, request.NailSurfaceId);
            if (validationError != null)
            {
                return new ApiErrorResult<NailVariantDto>(validationError);
            }

            _mapper.Map(request, variant);
            variant.Price = await CalculateNailVariantPriceAsync(request.NailShapeId, request.NailSurfaceId, id);
            variant.Duration = await CalculateNailVariantDurationAsync(request.NailShapeId, request.NailSurfaceId, id);
            _unitOfWork.NailVariantRepository.Update(variant);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailDesignPriceRangeAsync(previousNailDesignId);
            if (previousNailDesignId != variant.NailDesignId)
            {
                await UpdateNailDesignPriceRangeAsync(variant.NailDesignId);
            }

            var updatedVariant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(id);
            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(updatedVariant), "Cập nhật biến thể móng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailVariantAsync(int id)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(id);
            if (variant == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy biến thể móng.");
            }

            var nailDesignId = variant.NailDesignId;
            _unitOfWork.NailVariantRepository.Delete(variant);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailDesignPriceRangeAsync(nailDesignId);

            return new ApiSuccessResult<bool>(true, "Xóa biến thể móng thành công.");
        }

        private async Task<decimal> CalculateNailVariantPriceAsync(int? nailShapeId, int? nailSurfaceId, int? nailVariantId = null)
        {
            var nailShape = nailShapeId.HasValue
                ? await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId.Value)
                : null;
            var nailSurface = nailSurfaceId.HasValue
                ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value)
                : null;
            var componentPrice = 0m;

            if (nailVariantId.HasValue)
            {
                var variant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(nailVariantId.Value);
                componentPrice = variant?.NailComponents.Sum(nailComponent => nailComponent.Component.Price) ?? 0m;
            }

            return (nailShape?.Price ?? 0m) + (nailSurface?.Price ?? 0m) + componentPrice;
        }

        private async Task<int?> CalculateNailVariantDurationAsync(int? nailShapeId, int? nailSurfaceId, int? nailVariantId = null)
        {
            var nailShape = nailShapeId.HasValue
                ? await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId.Value)
                : null;
            var nailSurface = nailSurfaceId.HasValue
                ? await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId.Value)
                : null;
            var componentDuration = 0;

            if (nailVariantId.HasValue)
            {
                var variant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(nailVariantId.Value);
                componentDuration = variant?.NailComponents.Sum(nailComponent => nailComponent.Component.Duration ?? 0) ?? 0;
            }

            return (nailShape?.Duration ?? 0) + (nailSurface?.Duration ?? 0) + componentDuration;
        }

        private async Task UpdateNailDesignPriceRangeAsync(int nailDesignId)
        {
            var nailDesign = await _unitOfWork.NailDesignRepository.GetByIdAsync(nailDesignId);
            if (nailDesign == null)
            {
                return;
            }

            var variants = await _unitOfWork.NailVariantRepository.GetNailVariantsByDesignIdAsync(nailDesignId);
            nailDesign.MinPrice = variants.Any() ? variants.Min(variant => variant.Price) : 0m;
            nailDesign.MaxPrice = variants.Any() ? variants.Max(variant => variant.Price) : 0m;

            _unitOfWork.NailDesignRepository.Update(nailDesign);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<string?> ValidateReferencesAsync(int nailDesignId, int? nailShapeId, int? nailSurfaceId)
        {
            var design = await _unitOfWork.NailDesignRepository.GetByIdAsync(nailDesignId);
            if (design == null || design.Status == "InActive")
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

        public async Task<ApiResult<List<NailVariantDto>>> GetCapableNailVariantsAsync(Guid artistId)
        {
            var artist = await _unitOfWork.NailArtistRepository.GetByIdAsync(artistId);
            if (artist == null)
            {
                return new ApiErrorResult<List<NailVariantDto>>("Không tìm thấy thợ nail.");
            }

            var capableVariants = await _unitOfWork.NailVariantRepository.GetNailVariantsCapableByArtistAsync(artistId);
            var response = _mapper.Map<List<NailVariantDto>>(capableVariants);

            return new ApiSuccessResult<List<NailVariantDto>>(response, "Lấy danh sách mẫu móng thợ có thể thực hiện thành công.");
        }
    }
}
