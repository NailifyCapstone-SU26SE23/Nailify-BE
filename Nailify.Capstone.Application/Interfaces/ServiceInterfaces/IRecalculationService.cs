using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Services;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IRecalculationService
    {
        Task<ApiResult<NailVariantPriceRecalculationResponseDTO>> RecalculateAllAsync();
        Task<ApiResult<CustomerNailPriceRecalculationResponseDTO>> RecalculateAllCustomerNailsAsync();
        Task<ProcessAllBookingsResult> ProcessAllCompletedBookingsAsync();

    }
}
