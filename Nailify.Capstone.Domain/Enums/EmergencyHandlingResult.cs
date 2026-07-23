using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum EmergencyHandlingResult
    {
        Reassigned = 1, // Tự động đổi thợ cùng ca, giữ nguyên giờ 
        RescheduleSuggested = 2, //  // Tự động dời sang slot khác (+/- 30-60p) có thể tặng thêm voucher 15%
        Cancelled = 3 // Tự động hủy đơn & có thể hoàn cọc 100% và tặng voucher 20%
    }
}
