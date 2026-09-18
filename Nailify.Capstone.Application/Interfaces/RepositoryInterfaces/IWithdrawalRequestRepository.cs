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
    public interface IWithdrawalRequestRepository : IGenericRepository<WithdrawalRequest>
    {
        Task<PagedList<WithdrawalRequest>> GetPagedPendingAsync(int pageNumber, int pageSize);
        Task<PagedList<WithdrawalRequest>> GetPagedWithdrawalsAsync(WithdrawalStatus? status, int pageNumber, int pageSize);
    }
}
