using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class WalletTransaction
    {
        public Guid WalletTransactionId { get; set; } = Guid.NewGuid();
        public Guid WalletId { get; set; }
        /// <summary>
        ///  Số tiền của giao dịch.
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Số tiền trong ví ngay truớc khi thực hiện giao dịch. (BalanceBefore = BalanceAfter - Amount)
        /// </summary>
        public decimal BalanceBefore { get; set; }
        /// <summary>
        /// Số tiền trong ví ngay sau khi thực hiện giao dịch. (BalanceAfter = BalanceBefore + Amount)
        /// </summary>
        public decimal BalanceAfter { get; set; }
        public WalletTransactionType Type { get; set; } = WalletTransactionType.Deposit;
        public WalletTransactionStatus Status { get; set; } = WalletTransactionStatus.Completed;
        public string? ReferenceId { get; set; }
        public WalletReferenceType? ReferenceType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual CustomerWallet Wallet { get; set; }
    }
}
