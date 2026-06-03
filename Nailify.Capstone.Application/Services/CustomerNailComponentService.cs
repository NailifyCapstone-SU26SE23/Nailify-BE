using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class CustomerNailComponentService : ICustomerNailComponentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomerNailService _customerNailService;

        public CustomerNailComponentService(IUnitOfWork unitOfWork, IMapper mapper, ICustomerNailService customerNailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customerNailService = customerNailService;
        }

        public async Task<ApiResult<PagedList<CustomerNailComponentDto>>> GetPagedCustomerNailComponentsAsync(int pageNumber, int pageSize, int? customerNailId = null)
        {
            var pagedResult = await _unitOfWork.CustomerNailComponentRepository.GetPagedCustomerNailComponentsAsync(pageNumber, pageSize, customerNailId);
            var mappedItems = _mapper.Map<List<CustomerNailComponentDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<CustomerNailComponentDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<CustomerNailComponentDto>>(resultPagedList, "Lấy danh sách thành phần trên móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailComponentDto>> GetCustomerNailComponentByIdAsync(int id)
        {
            var customerNailComponent = await _unitOfWork.CustomerNailComponentRepository.GetCustomerNailComponentDetailAsync(id);
            if (customerNailComponent == null)
            {
                return new ApiErrorResult<CustomerNailComponentDto>("Không tìm thấy thành phần trên móng tùy chỉnh.");
            }

            return new ApiSuccessResult<CustomerNailComponentDto>(_mapper.Map<CustomerNailComponentDto>(customerNailComponent), "Lấy thông tin thành phần trên móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailComponentDto>> CreateCustomerNailComponentAsync(CustomerNailComponentCreateRequest request)
        {
            var validationError = await ValidateReferencesAsync(request.CustomerNailId, request.ComponentId, request.CustomerComponentId);
            if (validationError != null)
            {
                return new ApiErrorResult<CustomerNailComponentDto>(validationError);
            }

            var customerNailComponent = _mapper.Map<CustomerNailComponent>(request);
            await _unitOfWork.CustomerNailComponentRepository.CreateAsync(customerNailComponent);
            await _unitOfWork.SaveChangesAsync();
            await _customerNailService.RecalculateCustomerNailPriceAsync(request.CustomerNailId);

            var createdCustomerNailComponent = await _unitOfWork.CustomerNailComponentRepository.GetCustomerNailComponentDetailAsync(customerNailComponent.CustomerNailComponentId);
            return new ApiSuccessResult<CustomerNailComponentDto>(_mapper.Map<CustomerNailComponentDto>(createdCustomerNailComponent), "Tạo thành phần trên móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailComponentDto>> UpdateCustomerNailComponentAsync(CustomerNailComponentUpdateRequest request)
        {
            var customerNailComponent = await _unitOfWork.CustomerNailComponentRepository.GetByIdAsync(request.CustomerNailComponentId);
            if (customerNailComponent == null)
            {
                return new ApiErrorResult<CustomerNailComponentDto>("Không tìm thấy thành phần trên móng tùy chỉnh.");
            }

            var previousCustomerNailId = customerNailComponent.CustomerNailId;
            var validationError = await ValidateReferencesAsync(request.CustomerNailId, request.ComponentId, request.CustomerComponentId);
            if (validationError != null)
            {
                return new ApiErrorResult<CustomerNailComponentDto>(validationError);
            }

            _mapper.Map(request, customerNailComponent);
            _unitOfWork.CustomerNailComponentRepository.Update(customerNailComponent);
            await _unitOfWork.SaveChangesAsync();
            await _customerNailService.RecalculateCustomerNailPriceAsync(previousCustomerNailId);
            if (previousCustomerNailId != request.CustomerNailId)
            {
                await _customerNailService.RecalculateCustomerNailPriceAsync(request.CustomerNailId);
            }

            var updatedCustomerNailComponent = await _unitOfWork.CustomerNailComponentRepository.GetCustomerNailComponentDetailAsync(request.CustomerNailComponentId);
            return new ApiSuccessResult<CustomerNailComponentDto>(_mapper.Map<CustomerNailComponentDto>(updatedCustomerNailComponent), "Cập nhật thành phần trên móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<bool>> DeleteCustomerNailComponentAsync(int id)
        {
            var customerNailComponent = await _unitOfWork.CustomerNailComponentRepository.GetByIdAsync(id);
            if (customerNailComponent == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thành phần trên móng tùy chỉnh.");
            }

            var customerNailId = customerNailComponent.CustomerNailId;
            _unitOfWork.CustomerNailComponentRepository.Delete(customerNailComponent);
            await _unitOfWork.SaveChangesAsync();
            await _customerNailService.RecalculateCustomerNailPriceAsync(customerNailId);

            return new ApiSuccessResult<bool>(true, "Xóa thành phần trên móng tùy chỉnh thành công.");
        }

        private async Task<string?> ValidateReferencesAsync(int customerNailId, int? componentId, int? customerComponentId)
        {
            if (await _unitOfWork.CustomerNailRepository.GetByIdAsync(customerNailId) == null)
            {
                return "Không tìm thấy móng tùy chỉnh.";
            }

            if (componentId.HasValue == customerComponentId.HasValue)
            {
                return "Chỉ được chọn một trong Component hoặc CustomerComponent.";
            }

            if (componentId.HasValue && await _unitOfWork.ComponentRepository.GetByIdAsync(componentId.Value) == null)
            {
                return "Không tìm thấy component.";
            }

            if (customerComponentId.HasValue && await _unitOfWork.CustomerComponentRepository.GetByIdAsync(customerComponentId.Value) == null)
            {
                return "Không tìm thấy thành phần tùy chỉnh.";
            }

            return null;
        }
    }
}
