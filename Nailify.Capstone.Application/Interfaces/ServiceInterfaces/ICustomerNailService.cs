using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICustomerNailService
    {
        Task<ApiResult<PagedList<CustomerNailDto>>> GetPagedCustomerNailsAsync(int pageNumber, int pageSize, Guid? userId = null, string? name = null, bool? isPublic = null);
        Task<ApiResult<CustomerNailDto>> GetCustomerNailByIdAsync(int id);
        Task<ApiResult<CustomerNailDto>> CreateCustomerNailAsync(CustomerNailCreateRequest request, string? imageUrl = null, Guid? userId = null);
        Task<ApiResult<CustomerNailDto>> UpdateCustomerNailAsync(int id, CustomerNailUpdateRequest request, string? imageUrl = null);
        Task<ApiResult<bool>> DeleteCustomerNailAsync(int id);
        Task RecalculateCustomerNailPriceAsync(int customerNailId);
        Task<ApiResult<CustomerNailDto>> SubmitReviewAsync(int id, Guid customerId);
        Task<ApiResult<CustomerNailDto>> AssignReviewerAsync(int id, AssignArtistRequestDTO request);
        Task<ApiResult<CustomerNailDto>> ArtistQuoteAsync(int id, Guid artistId, ArtistQuoteRequestDTO request);
        Task<ApiResult<CustomerNailDto>> ManagerApproveQuoteAsync(int id, ManagerApproveQuoteRequestDTO request);
        Task<ApiResult<CustomerNailDto>> ManagerRejectRequestAsync(int id, RejectRequestDTO request);
        Task<ApiResult<CustomerNailDto>> CustomerRespondQuoteAsync(int id, Guid customerId, CustomerRespondQuoteRequest request);
    }
}
