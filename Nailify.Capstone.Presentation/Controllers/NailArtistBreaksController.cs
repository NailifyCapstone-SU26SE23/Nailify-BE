using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistBreakResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API Quản lý Lịch Nghỉ Giữa Ca của Thợ Nail (Staff Break).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NailArtistBreaksController : ControllerBase
    {
        private readonly INailArtistBreakService _breakService;
        public NailArtistBreaksController(INailArtistBreakService breakService)
        {
            _breakService = breakService;
        }
        /// <summary>
        /// Thợ nail gửi yêu cầu nghỉ đột xuất / việc riêng giữa ca (Mặc định ở trạng thái Chờ duyệt).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<NailArtistBreakResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] NailArtistBreakCreateRequestDTO request)
        {
            var result = await _breakService.CreateBreakAsync(request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Thợ nail cập nhật lại lịch nghỉ phép (Tự động chuyển về trạng thái Chờ duyệt).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailArtistBreakResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] NailArtistBreakUpdateRequestDTO request)
        {
            var result = await _breakService.UpdateBreakAsync(id, request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Thợ nail (hoặc Tiếp tân) hủy bỏ yêu cầu xin nghỉ (Xóa khỏi DB).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _breakService.DeleteBreakAsync(id);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Quản lý Salon duyệt hoặc từ chối yêu cầu xin nghỉ phép của Thợ Nail.
        /// </summary>
        [HttpPost("{id}/approve-reject")]
        [ProducesResponseType(typeof(ApiResult<NailArtistBreakResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveReject(
            Guid id,
            [FromQuery] ArtistBreakStatus status,
            [FromQuery] string? rejectReason = null)
        {
            var request = new ApproveRejectBreakRequest { Status = status, RejectReason = rejectReason };
            var result = await _breakService.ApproveRejectBreakAsync(id, request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Xem danh sách lịch xin nghỉ phép phân trang (Lọc theo thợ hoặc theo ngày cụ thể).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailArtistBreakResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? artistId = null,
            [FromQuery] DateTime? date = null,
            [FromQuery] ArtistBreakStatus? status = null,
    [FromQuery] string? orderBy = null)
        {
            var statusStr = (status == null) ? null : status.ToString();
            var result = await _breakService.GetPagedBreaksAsync(pageNumber, pageSize, artistId, date, statusStr, orderBy);
            return Ok(result);
        }
    }
}
