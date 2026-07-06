using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ChairRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ChairResponseDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IChairService
    {
        Task<ApiResult<PagedList<ChairResponseDTO>>> GetChairsBySalonAsync(Guid salonId, PagingRequestParameters parameters);
        Task<ApiResult<ChairResponseDTO>> GetChairByIdAsync(Guid id);
        Task<ApiResult<ChairResponseDTO>> CreateChairAsync(ChairCreateRequest request);
        Task<ApiResult<ChairResponseDTO>> UpdateChairAsync(Guid id, ChairUpdateRequest request);
        Task<ApiResult<bool>> DeleteChairAsync(Guid id);
        Task<ApiResult<List<ChairResponseDTO>>> GetAvailableChairsAsync(Guid salonId, DateTime bookingDate, TimeSpan startTime, int durationMinutes);
    }
}
