using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs
{
    public class SystemWalletSummaryDto
    {
        public decimal TotalUserBalance { get; set; }
        public decimal TotalFrozenBalance { get; set; }
        public int TotalActiveWallets { get; set; }
        public decimal TotalDepositedAmount { get; set; }
        public decimal TotalWithdrawnAmount { get; set; }
        public int PendingWithdrawalRequests { get; set; }
    }
}
