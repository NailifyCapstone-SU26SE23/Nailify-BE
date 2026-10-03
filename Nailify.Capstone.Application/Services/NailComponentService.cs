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
        private readonly IRecalculationService _recalculationService;

        public NailComponentService(IUnitOfWork unitOfWork, IMapper mapper, IRecalculationService recalculationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _recalculationService = recalculationService;
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
            await _recalculationService.RecalculateNailVariantAsync(request.NailVariantId);

            var createdNailComponent = await _unitOfWork.NailComponentRepository.GetNailComponentDetailAsync(nailComponent.NailComponentId);
            return new ApiSuccessResult<NailComponentDto>(_mapper.Map<NailComponentDto>(createdNailComponent), "Tạo thành phần móng thành công.");
        }

        public async Task<ApiResult<NailComponentDto>> UpdateNailComponentAsync(int id, NailComponentUpdateRequest request)
        {
            var nailComponent = await _unitOfWork.NailComponentRepository.GetByIdAsync(id);
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
            await _recalculationService.RecalculateNailVariantAsync(previousNailVariantId);
            if (previousNailVariantId != request.NailVariantId)
            {
                await _recalculationService.RecalculateNailVariantAsync(request.NailVariantId);
            }

            var updatedNailComponent = await _unitOfWork.NailComponentRepository.GetNailComponentDetailAsync(id);
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
            await _recalculationService.RecalculateNailVariantAsync(nailVariantId);

            return new ApiSuccessResult<bool>(true, "Xóa thành phần móng thành công.");
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
