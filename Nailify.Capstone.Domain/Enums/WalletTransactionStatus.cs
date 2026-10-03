using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum WalletTransactionStatus
    {
        Pending = 1,   // Giao dịch đang chờ xử lý
        Completed = 2, // Giao dịch đã hoàn tất
        Failed = 3,    // Giao dịch thất bại
        Cancelled = 4  // Giao dịch bị hủy
    }
}
