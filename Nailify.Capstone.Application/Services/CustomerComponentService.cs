using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class CustomerComponentService : ICustomerComponentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomerNailService _customerNailService;

        public CustomerComponentService(IUnitOfWork unitOfWork, IMapper mapper, ICustomerNailService customerNailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customerNailService = customerNailService;
        }

        public async Task<ApiResult<PagedList<CustomerComponentDto>>> GetPagedCustomerComponentsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, ComponentType? componentType = null)
        {
            var pagedResult = await _unitOfWork.CustomerComponentRepository.GetPagedCustomerComponentsAsync(pageNumber, pageSize, userId, name, componentType);
            var mappedItems = _mapper.Map<List<CustomerComponentDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<CustomerComponentDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<CustomerComponentDto>>(resultPagedList, "Lấy danh sách thành phần tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerComponentDto>> GetCustomerComponentByIdAsync(int id)
        {
            var customerComponent = await _unitOfWork.CustomerComponentRepository.GetByIdAsync(id);
            if (customerComponent == null)
            {
                return new ApiErrorResult<CustomerComponentDto>("Không tìm thấy thành phần tùy chỉnh.");
            }

            return new ApiSuccessResult<CustomerComponentDto>(_mapper.Map<CustomerComponentDto>(customerComponent), "Lấy thông tin thành phần tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerComponentDto>> CreateCustomerComponentAsync(CustomerComponentCreateRequest request, string? imageUrl = null, Guid? userId = null)
        {
                        var customerComponent = _mapper.Map<CustomerComponent>(request);
            customerComponent.ImageUrl = imageUrl ?? string.Empty;
            customerComponent.CreatedAt = DateTime.UtcNow;
            customerComponent.UserId = userId ?? Guid.Empty;
            await _unitOfWork.CustomerComponentRepository.CreateAsync(customerComponent);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<CustomerComponentDto>(_mapper.Map<CustomerComponentDto>(customerComponent), "Tạo thành phần tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerComponentDto>> UpdateCustomerComponentAsync(CustomerComponentUpdateRequest request)
        {
            var customerComponent = await _unitOfWork.CustomerComponentRepository.GetByIdAsync(request.CustomerComponentId);
            if (customerComponent == null)
            {
                return new ApiErrorResult<CustomerComponentDto>("Không tìm thấy thành phần tùy chỉnh.");
            }

            if (await _unitOfWork.UserRepository.GetByIdAsync(request.UserId) == null)
            {
                return new ApiErrorResult<CustomerComponentDto>("Không tìm thấy người dùng.");
            }

            _mapper.Map(request, customerComponent);
            _unitOfWork.CustomerComponentRepository.Update(customerComponent);
            await _unitOfWork.SaveChangesAsync();
            await RecalculateAffectedCustomerNailsAsync(request.CustomerComponentId);

            return new ApiSuccessResult<CustomerComponentDto>(_mapper.Map<CustomerComponentDto>(customerComponent), "Cập nhật thành phần tùy chỉnh thành công.");
        }

        public async Task<ApiResult<bool>> DeleteCustomerComponentAsync(int id)
        {
            var customerComponent = await _unitOfWork.CustomerComponentRepository.GetByIdAsync(id);
            if (customerComponent == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy thành phần tùy chỉnh.");
            }

            var affectedCustomerNailIds = await _unitOfWork.CustomerComponentRepository.GetCustomerNailIdsByCustomerComponentIdAsync(id);
            _unitOfWork.CustomerComponentRepository.Delete(customerComponent);
            await _unitOfWork.SaveChangesAsync();

            foreach (var customerNailId in affectedCustomerNailIds)
            {
                await _customerNailService.RecalculateCustomerNailPriceAsync(customerNailId);
            }

            return new ApiSuccessResult<bool>(true, "Xóa thành phần tùy chỉnh thành công.");
        }

        private async Task RecalculateAffectedCustomerNailsAsync(int customerComponentId)
        {
            var customerNailIds = await _unitOfWork.CustomerComponentRepository.GetCustomerNailIdsByCustomerComponentIdAsync(customerComponentId);
            foreach (var customerNailId in customerNailIds)
            {
                await _customerNailService.RecalculateCustomerNailPriceAsync(customerNailId);
            }
        }
    }
}
