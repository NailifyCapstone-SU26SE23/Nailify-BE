using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    public class WithdrawalRequest
    {
        public Guid WithdrawalRequestId { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid WalletId { get; set; }
        /// <summary>
        ///  Số tiền yêu cầu rút từ ví của khách hàng. (Amount phải nhỏ hơn hoặc bằng Balance trong CustomerWallet)
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        ///  Mã viết tắt ngân hàng (VCB, MB, ACB, VPB,...). Mã ngân hàng này sẽ được sử dụng để xác định ngân hàng mà khách hàng muốn rút tiền từ ví của mình.
        /// </summary>
        public string BankCode { get; set; } = null!;
        /// <summary>
        /// Tên ngân hàng (Vietcombank, MB Bank, ACB, VPBank,...). Tên ngân hàng này sẽ được sử dụng để xác định ngân hàng mà khách hàng muốn rút tiền từ ví của mình.
        /// </summary>
        public string BankName { get; set; } = null!;
        /// <summary>
        /// Số tài khoản ngân hàng của khách hàng. Số tài khoản này sẽ được sử dụng để xác định tài khoản ngân hàng mà khách hàng muốn rút tiền từ ví của mình.
        /// </summary>
        public string AccountNumber { get; set; } = null!;
        /// <summary>
        /// Tên chủ tài khoản ngân hàng của khách hàng. Tên này sẽ được sử dụng để xác định chủ tài khoản ngân hàng mà khách hàng muốn rút tiền từ ví của mình.
        /// </summary>
        public string AccountHolderName { get; set; } = null!;
        public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;
        public string? AdminNote { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? TransactionReference { get; set; }
        public virtual CustomerWallet Wallet { get; set; } = null!;

    }
}
