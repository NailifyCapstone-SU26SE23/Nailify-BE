using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface INailVariantPriceRecalculationService
    {
        Task<ApiResult<NailVariantPriceRecalculationResponseDTO>> RecalculateAllAsync();
        Task<ApiResult<CustomerNailPriceRecalculationResponseDTO>> RecalculateAllCustomerNailsAsync();
    }
}
