using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WaitlistRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WaitlistResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingWaitlistService
    {
        Task<ApiResult<WaitlistResponseDTO>> JoinWaitlistAsync(Guid customerId, JoinWaitlistRequestDTO request);
        Task<ApiResult<WaitlistResponseDTO>> ConfirmWaitlistAsync(Guid waitlistId, Guid customerId, ConfirmWaitlistRequestDTO request);
        Task<ApiResult<WaitlistResponseDTO>> CancelWaitlistAsync(Guid waitlistId, Guid customerId);
        Task<ApiResult<WaitlistResponseDTO>> GetMyWaitlistAsync(Guid customerId, Guid salonId);
        Task<ApiResult<List<WaitlistResponseDTO>>> GetMyWaitlistsAsync(Guid customerId);
        Task<ApiResult<PagedList<WaitlistResponseDTO>>> GetSalonWaitlistAsync(Guid salonId, int pageNumber, int pageSize);
    }
}
