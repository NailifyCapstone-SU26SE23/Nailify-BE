using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IBookingJobExecutor
    {
        // Quét và tự động hủy lịch đặt trễ quá 15 phút mà không Check-in
        Task CancelLateBookingsAsync();
        Task SendBookingReminderEmailAsync(Guid bookingId);
    }
}
