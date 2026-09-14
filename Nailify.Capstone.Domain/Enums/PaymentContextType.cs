using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum PaymentContextType
    {
        BookingPayment = 1, // Thanh toán
        BookingRequestPayment = 2, // Cọc
        WalletDeposit = 3, // Nạp tiền vào ví
    }
}
