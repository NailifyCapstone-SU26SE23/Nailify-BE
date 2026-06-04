using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ScheduleRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ScheduleResponseDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IScheduleService
    {
        Task<ApiResult<ScheduleResponseDTO>> CreateScheduleAsync(ScheduleCreateRequest request);
        Task<ApiResult<IEnumerable<ScheduleResponseDTO>>> GetSchedulesByArtistIdAsync(Guid artistId, DateTime? startDate, DateTime? endDate);
        Task<ApiResult<PagedList<ScheduleResponseDTO>>> GetPagedSchedulesAsync(int pageNumber, int pageSize, Guid? artistId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResult<ScheduleResponseDTO>> UpdateScheduleAsync(Guid scheduleId, ScheduleUpdateRequest request);
        Task<ApiResult<ScheduleResponseDTO>> PatchScheduleAsync(Guid scheduleId, SchedulePatchRequest request);
        Task<ApiResult<bool>> DeleteScheduleAsync(Guid scheduleId);
    }
}
