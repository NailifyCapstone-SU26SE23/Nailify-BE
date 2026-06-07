using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ISalonService
    {
        Task<ApiResult<PagedList<SalonResponseDTO>>> GetPagedSalonsAsync(SalonRequestParameters parameters);
        Task<ApiResult<SalonResponseDTO>> GetSalonByIdAsync(Guid id);
        Task<ApiResult<SalonResponseDTO>> CreateSalonAsync(SalonCreateRequest request, string? imageUrl = null);
        Task<ApiResult<SalonResponseDTO>> UpdateSalonAsync(Guid id, SalonUpdateRequest request, string? imageUrl = null);
        Task<ApiResult<SalonResponseDTO>> PatchSalonAsync(Guid id, SalonPatchRequest request, string? imageUrl = null);
        Task<ApiResult<bool>> DeleteSalonAsync(Guid id);
        Task<ApiResult<bool>> UpdateOperatingHoursAsync(Guid salonId, List<SalonOperatingHourUpdateRequest> operatingHours);
    }
}
