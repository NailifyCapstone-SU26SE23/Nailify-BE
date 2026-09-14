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
    public interface IWalletTransactionRepository : IGenericRepository<WalletTransaction>
    {
        Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(Guid walletId);
        Task<PagedList<WalletTransaction>> GetPagedByWalletIdAsync(Guid walletId, int pageNumber, int pageSize);
        Task<PagedList<WalletTransaction>> GetPagedSystemTransactionsAsync(WalletTransactionType? type, WalletTransactionStatus? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
    }
}
