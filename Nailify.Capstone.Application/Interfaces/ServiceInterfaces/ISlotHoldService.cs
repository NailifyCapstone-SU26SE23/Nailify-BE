using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface ISlotHoldService
    {
        /// <summary>
        ///Giữ chỗ slot.Trả về holdToken nếu thành công.
        /// </summary>
        Task<ApiResult<SlotHoldResponseDTO>> HoldSlotAsync(Guid customerId, HoldSlotRequestDTO request);
        /// <summary>
        /// Giải phóng slot hold (khách hủy giữ chỗ).
        /// </summary>
        Task<ApiResult<bool>> ReleaseSlotAsync(Guid customerId, string holdToken);
        /// <summary>
        /// Xem trạng thái hold (còn bao nhiêu giây).
        /// </summary>
        Task<ApiResult<SlotHoldResponseDTO>> GetHoldStatusAsync(string holdToken);
        /// <summary>
        /// Kiểm tra slot có đang bị giữ không
        /// </summary>
        Task<bool> IsSlotHeldAsync(Guid artistId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        /// <summary>
        /// Validate holdToken có hợp lệ, thuộc về customer và khớp với thông tin slot được chọn không.
        /// </summary>
        Task<bool> ValidateHoldTokenAsync(string holdToken, Guid customerId, Guid artistId, DateTime date, TimeSpan startTime);
        // <summary>
        /// Xóa hold sau khi booking tạo thành công. Slot đã có booking rồi → không cần hold nữa.
        /// </summary>
        Task ConsumeHoldAsync(string holdToken);
    }
}
