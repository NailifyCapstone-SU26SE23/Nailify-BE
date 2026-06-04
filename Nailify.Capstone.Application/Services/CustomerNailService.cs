using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Services
{
    public class CustomerNailService : ICustomerNailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerNailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<CustomerNailDto>>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, bool? isPublic = null, bool? isFavorite = null)
        {
            var pagedResult = await _unitOfWork.CustomerNailRepository.GetPagedCustomerNailsAsync(pageNumber, pageSize, userId, name, isPublic, isFavorite);
            var mappedItems = _mapper.Map<List<CustomerNailDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<CustomerNailDto>(mappedItems, pagedResult.MetaData.TotalItems, pageNumber, pageSize);

            return new ApiSuccessResult<PagedList<CustomerNailDto>>(resultPagedList, "Lấy danh sách móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> GetCustomerNailByIdAsync(int id)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(id);
            if (customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy móng tùy chỉnh.");
            }

            return new ApiSuccessResult<CustomerNailDto>(_mapper.Map<CustomerNailDto>(customerNail), "Lấy thông tin móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> CreateCustomerNailAsync(CustomerNailCreateRequest request, string? imageUrl = null, Guid? userId = null)
        {
            var validationError = await ValidateReferencesAsync(request.NailShapeId, request.NailSurfaceId, request.BasedOnNailVariantId);
            if (validationError != null)
            {
                return new ApiErrorResult<CustomerNailDto>(validationError);
            }

            var customerNail = _mapper.Map<CustomerNail>(request);
            customerNail.UserId = userId ?? Guid.Empty;
            customerNail.ImageUrl = imageUrl ?? string.Empty;
            customerNail.CreatedAt = DateTime.UtcNow;
            customerNail.Price = await CalculateCustomerNailPriceAsync(request.NailShapeId, request.NailSurfaceId, null);

            await _unitOfWork.CustomerNailRepository.CreateAsync(customerNail);
            await _unitOfWork.SaveChangesAsync();

            var createdCustomerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNail.CustomerNailId);
            return new ApiSuccessResult<CustomerNailDto>(_mapper.Map<CustomerNailDto>(createdCustomerNail), "Tạo móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<CustomerNailDto>> UpdateCustomerNailAsync(CustomerNailUpdateRequest request)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(request.CustomerNailId);
            if (customerNail == null)
            {
                return new ApiErrorResult<CustomerNailDto>("Không tìm thấy móng tùy chỉnh.");
            }

            var validationError = await ValidateReferencesAsync(request.NailShapeId, request.NailSurfaceId, request.BasedOnNailVariantId);
            if (validationError != null)
            {
                return new ApiErrorResult<CustomerNailDto>(validationError);
            }

            _mapper.Map(request, customerNail);
            customerNail.Price = await CalculateCustomerNailPriceAsync(request.NailShapeId, request.NailSurfaceId, request.CustomerNailId);

            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();

            var updatedCustomerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(request.CustomerNailId);
            return new ApiSuccessResult<CustomerNailDto>(_mapper.Map<CustomerNailDto>(updatedCustomerNail), "Cập nhật móng tùy chỉnh thành công.");
        }

        public async Task<ApiResult<bool>> DeleteCustomerNailAsync(int id)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(id);
            if (customerNail == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy móng tùy chỉnh.");
            }

            _unitOfWork.CustomerNailRepository.Delete(customerNail);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa móng tùy chỉnh thành công.");
        }

        public async Task RecalculateCustomerNailPriceAsync(int customerNailId)
        {
            var customerNail = await _unitOfWork.CustomerNailRepository.GetByIdAsync(customerNailId);
            if (customerNail == null)
            {
                return;
            }

            customerNail.Price = await CalculateCustomerNailPriceAsync(customerNail.NailShapeId, customerNail.NailSurfaceId, customerNailId);
            _unitOfWork.CustomerNailRepository.Update(customerNail);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateCustomerNailPriceAsync(int nailShapeId, int nailSurfaceId, int? customerNailId)
        {
            var nailShape = await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId);
            var nailSurface = await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId);
            var componentPrice = 0m;

            if (customerNailId.HasValue)
            {
                var customerNail = await _unitOfWork.CustomerNailRepository.GetCustomerNailDetailAsync(customerNailId.Value);
                componentPrice = customerNail?.CustomerNailComponents.Sum(component =>
                    (component.Component?.Price ?? 0m) + (component.CustomerComponent?.Price ?? 0m)) ?? 0m;
            }

            return (nailShape?.Price ?? 0m) + (nailSurface?.Price ?? 0m) + componentPrice;
        }

        private async Task<string?> ValidateReferencesAsync(int nailShapeId, int nailSurfaceId, int? basedOnNailVariantId)
        {
            if (await _unitOfWork.NailShapeRepository.GetByIdAsync(nailShapeId) == null)
            {
                return "Không tìm thấy dáng móng.";
            }

            if (await _unitOfWork.NailSurfaceRepository.GetByIdAsync(nailSurfaceId) == null)
            {
                return "Không tìm thấy bề mặt móng.";
            }

            if (basedOnNailVariantId.HasValue && await _unitOfWork.NailVariantRepository.GetByIdAsync(basedOnNailVariantId.Value) == null)
            {
                return "Không tìm thấy biến thể móng gốc.";
            }

            return null;
        }
    }
}
