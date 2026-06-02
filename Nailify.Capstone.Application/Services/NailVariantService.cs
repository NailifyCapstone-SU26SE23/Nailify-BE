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
            await _unitOfWork.NailVariantRepository.CreateAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            var createdVariant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(variant.NailVariantId);
            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(createdVariant), "Tạo biến thể móng thành công.");
        }

        public async Task<ApiResult<NailVariantDto>> UpdateNailVariantAsync(NailVariantUpdateRequest request)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(request.NailVariantId);
            if (variant == null)
            {
                return new ApiErrorResult<NailVariantDto>("Không tìm thấy biến thể móng.");
            }

            var validationError = await ValidateReferencesAsync(request.NailDesignId, request.NailShapeId, request.NailSurfaceId);
            if (validationError != null)
            {
                return new ApiErrorResult<NailVariantDto>(validationError);
            }

            _mapper.Map(request, variant);
            _unitOfWork.NailVariantRepository.Update(variant);
            await _unitOfWork.SaveChangesAsync();

            var updatedVariant = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(request.NailVariantId);
            return new ApiSuccessResult<NailVariantDto>(_mapper.Map<NailVariantDto>(updatedVariant), "Cập nhật biến thể móng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailVariantAsync(int id)
        {
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(id);
            if (variant == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy biến thể móng.");
            }

            _unitOfWork.NailVariantRepository.Delete(variant);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa biến thể móng thành công.");
        }

        private async Task<string?> ValidateReferencesAsync(int nailDesignId, int nailShapeId, int nailSurfaceId)
        {
            var design = await _unitOfWork.NailDesignRepository.GetByIdAsync(nailDesignId);
            if (design == null || design.Status == "InActive")
            {
                return "Không tìm thấy mẫu nail.";
            }

            if (await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId) == null)
            {
                return "Không tìm thấy dáng móng.";
            }

            if (await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId) == null)
            {
                return "Không tìm thấy bề mặt móng.";
            }

            return null;
        }
    }
}
