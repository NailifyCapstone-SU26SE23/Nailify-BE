using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum StaffTransferStatus
    {
        Scheduled, // Đã lên lịch / đang hiệu lực (theo ngày)
        Completed, // Đã qua EndDate (job hoặc set khi query)
        Cancelled  // Bị hủy trước/giữa chừng
    }
}
