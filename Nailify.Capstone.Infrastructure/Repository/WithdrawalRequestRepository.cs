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
    public class WithdrawalRequestRepository : GenericRepository<WithdrawalRequest>, IWithdrawalRequestRepository
    {
        public WithdrawalRequestRepository(NailifyDbContext context) : base(context) { }
        public async Task<PagedList<WithdrawalRequest>> GetPagedPendingAsync(int pageNumber, int pageSize)
        {
            var query = _dbSet.AsNoTracking().Where(r => r.Status == WithdrawalStatus.Pending);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedList<WithdrawalRequest>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedList<WithdrawalRequest>> GetPagedWithdrawalsAsync(WithdrawalStatus? status, int pageNumber, int pageSize)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedList<WithdrawalRequest>(items, totalCount, pageNumber, pageSize);
        }
    }
}
