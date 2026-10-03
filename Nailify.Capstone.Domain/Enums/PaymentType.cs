using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum PaymentType
    {
        [Display(Name = "Đặt cọc lịch hẹn")]
        BookingDeposit = 1,
        [Display(Name = "Thanh toán phần tiền còn lại của lịch hẹn")]
        BookingRemaining = 2,
        [Display(Name = "Nạp tiền vào ví")]
        WalletDeposit = 3,
        [Display(Name = "Rút tiền từ ví về ngân hàng (Payout)")]
        WalletWithdrawal = 4
    }
}
