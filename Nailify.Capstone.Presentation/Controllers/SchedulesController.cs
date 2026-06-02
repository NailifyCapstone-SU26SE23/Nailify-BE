using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ScheduleRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ScheduleResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Lấy danh sách lịch trình phân trang (có thể lọc theo thợ nail và khoảng ngày).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<ScheduleResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] Guid? artistId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _scheduleService.GetPagedSchedulesAsync(pageNumber, pageSize, artistId, startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Lấy toàn bộ lịch trình của một thợ làm móng trong một khoảng thời gian (phục vụ hiển thị lịch).
        /// </summary>
        [HttpGet("artist/{artistId}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<ScheduleResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSchedulesByArtistId(
            Guid artistId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _scheduleService.GetSchedulesByArtistIdAsync(artistId, startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới một ca làm việc (Schedule) cho thợ làm móng.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<ScheduleResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateRequest request)
        {
            var result = await _scheduleService.CreateScheduleAsync(request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật toàn bộ thông tin một ca làm việc.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<ScheduleResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ScheduleUpdateRequest request)
        {
            var result = await _scheduleService.UpdateScheduleAsync(id, request);
            if (!result.IsSucceeded) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật một phần ca làm việc (PATCH).
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<ScheduleResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(Guid id, [FromBody] SchedulePatchRequest request)
        {
            var result = await _scheduleService.PatchScheduleAsync(id, request);
            if (!result.IsSucceeded) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Xóa một ca làm việc.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (!result.IsSucceeded) return NotFound(result);
            return Ok(result);
        }
    }
}
