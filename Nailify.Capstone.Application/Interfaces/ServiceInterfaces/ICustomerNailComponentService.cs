using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailComponentRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICustomerNailComponentService
    {
        Task<ApiResult<PagedList<CustomerNailComponentDto>>> GetPagedCustomerNailComponentsAsync(int pageNumber, int pageSize, int? customerNailId = null);
        Task<ApiResult<CustomerNailComponentDto>> GetCustomerNailComponentByIdAsync(int id);
        Task<ApiResult<CustomerNailComponentDto>> CreateCustomerNailComponentAsync(CustomerNailComponentCreateRequest request);
        Task<ApiResult<CustomerNailComponentDto>> UpdateCustomerNailComponentAsync(CustomerNailComponentUpdateRequest request);
        Task<ApiResult<bool>> DeleteCustomerNailComponentAsync(int id);
    }
}
