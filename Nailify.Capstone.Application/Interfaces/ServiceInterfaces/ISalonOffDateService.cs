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
    public interface ISalonOffDateService
    {
        Task<ApiResult<SalonOffDateResponseDTO>> AddSalonOffDateAsync(Guid salonId, CreateSalonOffDateRequestDTO request);
        Task<ApiResult<List<SalonOffDateResponseDTO>>> GetSalonOffDatesAsync(Guid salonId);
        Task<ApiResult<SalonOffDateResponseDTO>> UpdateSalonOffDateAsync(Guid salonOffDateId, UpdateSalonOffDateRequestDTO request);

        Task<ApiResult<bool>> DeleteSalonOffDateAsync(Guid salonOffDateId);
    }
}
