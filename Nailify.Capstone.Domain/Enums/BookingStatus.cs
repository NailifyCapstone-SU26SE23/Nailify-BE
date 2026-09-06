using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum BookingStatus
    {
        Pending,
        //Assigned,
        //Reviewed,
        Approved,
        Rejected,
        Cancelled,
        CheckedIn,
        InProgress,
        ServiceCompleted, // Thợ làm xong
        Completed, // Khách đã thanh toán và check-out hoàn toàn
        Repaired, // Đơn bảo hành đã hoàn thành
        ReschedulePending,   // Khách yêu cầu đổi lịch, chờ quản lý duyệt
        RescheduleSuggested   // Quản lý đề xuất giờ mới, chờ khách duyệt
    }
}
    