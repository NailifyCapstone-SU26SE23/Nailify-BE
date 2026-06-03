using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICustomerComponentService
    {
        Task<ApiResult<PagedList<CustomerComponentDto>>> GetPagedCustomerComponentsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, ComponentType? componentType = null);
        Task<ApiResult<CustomerComponentDto>> GetCustomerComponentByIdAsync(int id);
        Task<ApiResult<CustomerComponentDto>> CreateCustomerComponentAsync(CustomerComponentCreateRequest request, string? imageUrl = null);
        Task<ApiResult<CustomerComponentDto>> UpdateCustomerComponentAsync(CustomerComponentUpdateRequest request);
        Task<ApiResult<bool>> DeleteCustomerComponentAsync(int id);
    }
}
