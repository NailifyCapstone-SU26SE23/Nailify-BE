using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Common.Helpers
{
    public static class SalonOperatingHourHelper
    {
        public static bool IsWithinOperatingHours(this IEnumerable<SalonOperatingHour> operatingHours, TimeSpan start, TimeSpan end)
        {
            if(operatingHours == null || !operatingHours.Any())
            {
                return false;
            }
            // Neu do la ngay dong cua
            if(operatingHours.Any(x => x.IsClosed))
            {
                return false;
            }

            // Tìm giờ đóng cửa muộn nhất trong ngày của Salon
            var finalCloseTime = operatingHours.Max(x => x.CloseTime);

            // Giờ kết thúc dịch vụ bắt buộc không được vượt quá giờ đóng cửa cuối ngày
            if(end > finalCloseTime)
            {
                return false;
            }

            var response = operatingHours.Any(x => start >= x.OpenTime && start < x.CloseTime);
            return response;
        }
    }
}
