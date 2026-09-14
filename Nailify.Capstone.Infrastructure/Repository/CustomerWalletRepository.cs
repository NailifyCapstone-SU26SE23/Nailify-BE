using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
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
    public class CustomerWalletRepository : GenericRepository<CustomerWallet>, ICustomerWalletRepository
    {
        public CustomerWalletRepository(NailifyDbContext context) : base(context)
        {
        }

        public async Task<CustomerWallet?> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public async Task<CustomerWallet?> GetByCustomerIdForUpdateAsync(Guid customerId)
        {
            return await _context.Set<CustomerWallet>()
                      .FromSqlRaw("SELECT * FROM \"CustomerWallets\" WHERE \"CustomerId\" = {0} FOR UPDATE", customerId)
                      .FirstOrDefaultAsync();
        }

        public async Task<CustomerWallet?> GetByWalletIdForUpdateAsync(Guid walletId)
        {
            return await _context.Set<CustomerWallet>()
             .FromSqlRaw("SELECT * FROM \"CustomerWallets\" WHERE \"WalletId\" = {0} FOR UPDATE", walletId)
             .FirstOrDefaultAsync();
        }

        public async Task<SystemWalletSummaryDto> GetSystemSummaryAsync()
        {
            var totalUserBalance = await _dbSet.SumAsync(w => w.Balance);
            var totalFrozenBalance = await _dbSet.SumAsync(w => w.FrozenBalance);
            var totalActiveWallets = await _dbSet.CountAsync(w => w.Status == WalletStatus.Active);
            var totalDeposits = await _context.Set<WalletTransaction>()
                .Where(t => t.Type == WalletTransactionType.Deposit && t.Status == WalletTransactionStatus.Completed)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var totalWithdrawals = await _context.Set<WithdrawalRequest>()
                .Where(r => r.Status == WithdrawalStatus.Approved)
                .SumAsync(r => (decimal?)r.Amount) ?? 0m;
            var pendingCount = await _context.Set<WithdrawalRequest>()
                .CountAsync(r => r.Status == WithdrawalStatus.Pending);
            return new SystemWalletSummaryDto
            {
                TotalUserBalance = totalUserBalance,
                TotalFrozenBalance = totalFrozenBalance,
                TotalActiveWallets = totalActiveWallets,
                TotalDepositedAmount = totalDeposits,
                TotalWithdrawnAmount = totalWithdrawals,
                PendingWithdrawalRequests = pendingCount
            };
        }
    }
}
