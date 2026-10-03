using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class CustomerWallet
    {
        public Guid WalletId { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        /// Tổng số dư tiền mặt hiện có trong Ví
        public decimal Balance { get; set; } = 0m;
        /// Số dư bị đóng băng (đang chờ xử lý, ví dụ: khi khách hàng đặt cọc hoặc thanh toán cho một dịch vụ)
        public decimal FrozenBalance { get; set; } = 0m;
        public WalletStatus Status { get; set; } = WalletStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
        public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();
    }
}
