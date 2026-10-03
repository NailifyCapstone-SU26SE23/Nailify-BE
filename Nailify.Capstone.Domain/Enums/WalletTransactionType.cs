using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum WalletTransactionType
    {
        Deposit = 1,  // Nạp tiền vào ví (+)
        Withdraw = 2, // Rut tiền từ ví (-)
        BookingPayment = 3, // Thanh toán cho booking (-)
        BookingRefund = 4, // Hoàn tiền cho booking (+)
        ConvertToPoints = 5 // Chuyển đổi tiền thành điểm (-)
    }
}
