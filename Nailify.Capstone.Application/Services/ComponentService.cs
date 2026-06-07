using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class ComponentService : IComponentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ComponentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<ComponentDto>>> GetPagedComponentsAsync(int pageNumber, int pageSize, string? name = null, ComponentType? componentType = null)
        {
            var pagedResult = await _unitOfWork.ComponentRepository.GetPagedComponentsAsync(pageNumber, pageSize, name, componentType);
            var mappedItems = _mapper.Map<List<ComponentDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<ComponentDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<ComponentDto>>(resultPagedList, "Lấy danh sách thành phần thành công.");
        }

        public async Task<ApiResult<ComponentDto>> GetComponentByIdAsync(int id)
        {
            var component = await _unitOfWork.ComponentRepository.GetByIdAsync(id);
            if (component == null)
            {
                return new ApiErrorResult<ComponentDto>("Không tìm thấy thành phần.");
            }

            return new ApiSuccessResult<ComponentDto>(_mapper.Map<ComponentDto>(component), "Lấy thông tin thành phần thành công.");
        }

        public async Task<ApiResult<ComponentDto>> CreateComponentAsync(ComponentCreateRequest request, string? imageUrl = null)
        {
            var component = _mapper.Map<Component>(request);
            component.ImageUrl = imageUrl ?? string.Empty;
            await _unitOfWork.ComponentRepository.CreateAsync(component);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<ComponentDto>(_mapper.Map<ComponentDto>(component), "Tạo thành phần thành công.");
        }

        public async Task<ApiResult<ComponentDto>> UpdateComponentAsync(int id, ComponentUpdateRequest request)
        {
            var component = await _unitOfWork.ComponentRepository.GetByIdAsync(id);
            if (component == null)
            {
                return new ApiErrorResult<ComponentDto>("Không tìm thấy thành phần.");
            }

            _mapper.Map(request, component);
            _unitOfWork.ComponentRepository.Update(component);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<ComponentDto>(_mapper.Map<ComponentDto>(component), "Cập nhật thành phần thành công.");
        }

        public async Task<ApiResult<bool>> DeleteComponentAsync(int id)
        {
            var component = await _unitOfWork.ComponentRepository.GetByIdAsync(id);
            if (component == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thành phần.");
            }

            _unitOfWork.ComponentRepository.Delete(component);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa thành phần thành công.");
        }
    }
}
