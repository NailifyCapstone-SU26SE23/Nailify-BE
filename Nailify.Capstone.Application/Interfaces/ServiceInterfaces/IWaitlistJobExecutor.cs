using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IWaitlistJobExecutor
    {
        // Hủy lượt waitlist nếu hết 15 phút đếm ước mà khách không xác nhận
        Task CancelIfExpiredAsync(Guid waitlistId);
        // Tác vụ dọn dẹp hàng chờ lúc 0h sáng hàng ngày (Option)
        Task ClearDailyWaitlistAsync();
    }
}
