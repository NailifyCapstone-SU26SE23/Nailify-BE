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
        Assigned,
        Reviewed,
        Approved,
        Rejected,
        Cancelled,
        CheckedIn,
        InProgress,
        ServiceCompleted, // Thợ làm xong
        Completed, // Khách đã thanh toán và check-out hoàn toàn
        Repaired
    }
}
    