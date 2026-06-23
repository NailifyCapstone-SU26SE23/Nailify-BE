using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICustomerNailRequestRepository : IGenericRepository<CustomerNailRequest>
    {
        Task<PagedList<CustomerNailRequest>> GetPagedCustomerNailRequestsAsync(
            int pageNumber, int pageSize, Guid? salonId = null, CustomerNailStatus? status = null, Guid? customerId = null, Guid? approvedArtistId = null);

        Task<CustomerNailRequest?> GetCustomerNailRequestDetailAsync(Guid requestId);
        Task<CustomerNailRequest?> GetApprovedRequestAsync(int customerNailId, Guid salonId);
        Task<CustomerNailRequest?> GetAnyApprovedRequestAsync(int customerNailId);
    }
}
