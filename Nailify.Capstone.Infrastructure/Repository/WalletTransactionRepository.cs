using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(Guid walletId)
        {
            return await _dbSet.Where(t => t.WalletId == walletId)
                          .OrderByDescending(t => t.CreatedAt)
                          .ToListAsync();
        }

        public async Task<PagedList<WalletTransaction>> GetPagedByWalletIdAsync(Guid walletId, int pageNumber, int pageSize)
        {
            var query = _dbSet.AsNoTracking().Where(t => t.WalletId == walletId);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedList<WalletTransaction>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedList<WalletTransaction>> GetPagedSystemTransactionsAsync(
            WalletTransactionType? type,
            WalletTransactionStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }
            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<WalletTransaction>(items, totalCount, pageNumber, pageSize);
        }
    }
}
