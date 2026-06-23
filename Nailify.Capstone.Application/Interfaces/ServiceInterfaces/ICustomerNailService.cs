using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.CustomerNailRequestResponseDTO;
using Nailify.Capstone.Domain.Enums;

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
        Task<ApiResult<CustomerNailRequestResponseDTO>> SubmitReviewAsync(CustomerNailRequestCreateRequest requestDto, Guid customerId);
        Task<ApiResult<CustomerNailRequestResponseDTO>> AssignReviewerAsync(Guid id, Guid managerUserId, AssignArtistRequestDTO request);
        Task<ApiResult<CustomerNailRequestResponseDTO>> ArtistQuoteAsync(Guid id, Guid artistAccountId, ArtistQuoteRequestDTO request);
        Task<ApiResult<CustomerNailRequestResponseDTO>> ManagerApproveQuoteAsync(Guid id, Guid managerUserId, ManagerApproveQuoteRequestDTO request);
        Task<ApiResult<CustomerNailRequestResponseDTO>> ManagerRejectRequestAsync(Guid id, Guid managerUserId, RejectRequestDTO request);
        Task<ApiResult<CustomerNailRequestResponseDTO>> CustomerRespondQuoteAsync(Guid id, Guid customerId, CustomerRespondQuoteRequest request);
    }
}
