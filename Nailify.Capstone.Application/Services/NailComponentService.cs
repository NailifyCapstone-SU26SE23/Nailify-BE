using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class NailComponentService : INailComponentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NailComponentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<NailComponentDto>>> GetPagedNailComponentsAsync(int pageNumber, int pageSize)
        {
            var pagedResult = await _unitOfWork.NailComponentRepository.GetPagedNailComponentsAsync(pageNumber, pageSize);
            var mappedItems = _mapper.Map<List<NailComponentDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<NailComponentDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<NailComponentDto>>(resultPagedList, "Lấy danh sách thành phần móng thành công.");
        }

        public async Task<ApiResult<NailComponentDto>> GetNailComponentByIdAsync(int id)
        {
            var nailComponent = await _unitOfWork.NailComponentRepository.GetNailComponentDetailAsync(id);
            if (nailComponent == null)
            {
                return new ApiErrorResult<NailComponentDto>("Không tìm thấy thành phần móng.");
            }

            return new ApiSuccessResult<NailComponentDto>(_mapper.Map<NailComponentDto>(nailComponent), "Lấy thông tin thành phần móng thành công.");
        }

        public async Task<ApiResult<NailComponentDto>> CreateNailComponentAsync(NailComponentCreateRequest request)
        {
            var validationError = await ValidateReferencesAsync(request.ComponentId, request.NailVariantId);
            if (validationError != null)
            {
                return new ApiErrorResult<NailComponentDto>(validationError);
            }

            var nailComponent = _mapper.Map<NailComponent>(request);
            await _unitOfWork.NailComponentRepository.CreateAsync(nailComponent);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailVariantAndDesignPriceAsync(request.NailVariantId);

            var createdNailComponent = await _unitOfWork.NailComponentRepository.GetNailComponentDetailAsync(nailComponent.NailComponentId);
            return new ApiSuccessResult<NailComponentDto>(_mapper.Map<NailComponentDto>(createdNailComponent), "Tạo thành phần móng thành công.");
        }

        public async Task<ApiResult<NailComponentDto>> UpdateNailComponentAsync(NailComponentUpdateRequest request)
        {
            var nailComponent = await _unitOfWork.NailComponentRepository.GetByIdAsync(request.NailComponentId);
            if (nailComponent == null)
            {
                return new ApiErrorResult<NailComponentDto>("Không tìm thấy thành phần móng.");
            }

            var previousNailVariantId = nailComponent.NailVariantId;
            var validationError = await ValidateReferencesAsync(request.ComponentId, request.NailVariantId);
            if (validationError != null)
            {
                return new ApiErrorResult<NailComponentDto>(validationError);
            }

            _mapper.Map(request, nailComponent);
            _unitOfWork.NailComponentRepository.Update(nailComponent);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailVariantAndDesignPriceAsync(previousNailVariantId);
            if (previousNailVariantId != request.NailVariantId)
            {
                await UpdateNailVariantAndDesignPriceAsync(request.NailVariantId);
            }

            var updatedNailComponent = await _unitOfWork.NailComponentRepository.GetNailComponentDetailAsync(request.NailComponentId);
            return new ApiSuccessResult<NailComponentDto>(_mapper.Map<NailComponentDto>(updatedNailComponent), "Cập nhật thành phần móng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteNailComponentAsync(int id)
        {
            var nailComponent = await _unitOfWork.NailComponentRepository.GetByIdAsync(id);
            if (nailComponent == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thành phần móng.");
            }

            var nailVariantId = nailComponent.NailVariantId;
            _unitOfWork.NailComponentRepository.Delete(nailComponent);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailVariantAndDesignPriceAsync(nailVariantId);

            return new ApiSuccessResult<bool>(true, "Xóa thành phần móng thành công.");
        }

        private async Task UpdateNailVariantAndDesignPriceAsync(int nailVariantId)
        {
            var variantDetail = await _unitOfWork.NailVariantRepository.GetNailVariantDetailAsync(nailVariantId);
            var variant = await _unitOfWork.NailVariantRepository.GetByIdAsync(nailVariantId);
            if (variantDetail == null || variant == null)
            {
                return;
            }

            variant.Price = variantDetail.NailShape.Price
                + variantDetail.NailSurface.Price
                + variantDetail.NailComponents.Sum(nailComponent => nailComponent.Component.Price);

            _unitOfWork.NailVariantRepository.Update(variant);
            await _unitOfWork.SaveChangesAsync();
            await UpdateNailDesignPriceRangeAsync(variant.NailDesignId);
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

        private async Task<string?> ValidateReferencesAsync(int componentId, int nailVariantId)
        {
            if (await _unitOfWork.ComponentRepository.GetByIdAsync(componentId) == null)
            {
                return "Không tìm thấy component.";
            }

            if (await _unitOfWork.NailVariantRepository.GetByIdAsync(nailVariantId) == null)
            {
                return "Không tìm thấy biến thể móng.";
            }

            return null;
        }
    }
}
