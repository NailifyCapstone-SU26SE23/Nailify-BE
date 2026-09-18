using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    public interface ICustomerWalletRepository : IGenericRepository<CustomerWallet>
    {
        Task<CustomerWallet?> GetByWalletIdForUpdateAsync(Guid walletId);
        Task<CustomerWallet?> GetByWalletIdWithCustomerSummaryAsync(Guid walletId);
        Task<CustomerWallet?> GetByCustomerIdForUpdateAsync(Guid customerId);
        Task<CustomerWallet?> GetByCustomerIdAsync(Guid customerId);
        Task<SystemWalletSummaryDto> GetSystemSummaryAsync();
    }
}
