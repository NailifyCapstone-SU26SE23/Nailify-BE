using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICustomerNailService
    {
        Task<ApiResult<PagedList<CustomerNailDto>>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, bool? isPublic = null, bool? isFavorite = null);
        Task<ApiResult<CustomerNailDto>> GetCustomerNailByIdAsync(int id);
        Task<ApiResult<CustomerNailDto>> CreateCustomerNailAsync(CustomerNailCreateRequest request, string? imageUrl = null, Guid? userId = null);
        Task<ApiResult<CustomerNailDto>> UpdateCustomerNailAsync(CustomerNailUpdateRequest request);
        Task<ApiResult<bool>> DeleteCustomerNailAsync(int id);
        Task RecalculateCustomerNailPriceAsync(int customerNailId);
    }
}
