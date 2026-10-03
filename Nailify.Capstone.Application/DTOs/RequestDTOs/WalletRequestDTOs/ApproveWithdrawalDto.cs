using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs
{
    public class ApproveWithdrawalDto
    {
        public string? AdminNote { get; set; }
        public string? TransactionReference { get; set; } // Mã giao dịch ngân hàng Payout (nếu có)
    }
}
