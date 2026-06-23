using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.CustomerNailRequestResponseDTO;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ICustomerNailRequestsService
    {
        Task<ApiResult<PagedList<CustomerNailRequestResponseDTO>>> GetPagedCustomerNailRequestsAsync(int pageNumber,int pageSize,Guid? salonId = null,CustomerNailStatus? status = null, Guid? customerId = null, Guid? approvedArtistId = null);
        Task<ApiResult<CustomerNailRequestResponseDTO>> GetCustomerNailRequestByIdAsync(Guid requestId);
    }
}
