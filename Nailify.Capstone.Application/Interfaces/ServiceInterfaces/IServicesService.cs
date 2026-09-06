using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ServiceRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ServiceResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IServicesService
    {
        Task<ApiResult<PagedList<ServiceResponseDTO>>> GetPagedServicesAsync(int pageNumber, int pageSize, string? searchName = null, string? status = null,
          string? orderBy = null);
        Task<ApiResult<ServiceResponseDTO>> GetServiceByIdAsync(Guid serviceId);
        Task<ApiResult<ServiceResponseDTO>> CreateServiceAsync(ServiceCreateRequestDTO request);
        Task<ApiResult<ServiceResponseDTO>> UpdateServiceAsync(Guid serviceId, ServiceUpdateRequestDTO request);
        Task<ApiResult<bool>> DeleteServiceAsync(Guid serviceId);
    }
}
