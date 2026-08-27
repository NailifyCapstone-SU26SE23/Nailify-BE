using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ChairRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ChairResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    public class ChairsController : ControllerBase
    {
        private readonly IChairService _chairService;

        public ChairsController(IChairService chairService)
        {
            _chairService = chairService;
        }

        /// <summary>
        /// Lấy danh sách ghế của chi nhánh Salon (phân trang).
        /// </summary>
        [HttpGet("api/salons/{salonId}/chairs")]
        [ProducesResponseType(typeof(ApiResult<PagedList<ChairResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChairsBySalon(Guid salonId, [FromQuery] PagingRequestParameters parameters)
        {
            var response = await _chairService.GetChairsBySalonAsync(salonId, parameters);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách các ghế còn trống của chi nhánh tại một thời điểm cụ thể.
        /// </summary>
        [HttpGet("api/salons/{salonId}/available-chairs")]
        [ProducesResponseType(typeof(ApiResult<List<ChairResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvailableChairs(
            Guid salonId,
            [FromQuery] DateTime bookingDate,
            [FromQuery] TimeSpan startTime,
            [FromQuery] int duration)
        {
            var response = await _chairService.GetAvailableChairsAsync(salonId, bookingDate, startTime, duration);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Lấy tất cả ghế của salon kèm trạng thái bận/trống tại một thời điểm.
        /// Ghế bận sẽ trả về BookingId, CustomerId và tên khách đang ngồi.
        /// </summary>
        [HttpGet("api/salons/{salonId}/chairs-status")]
        [ProducesResponseType(typeof(ApiResult<List<ChairResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChairStatus(
            Guid salonId,
            [FromQuery] DateTime? atDate = null,
            [FromQuery] TimeSpan? atTime = null)
        {
            var localNow = DateTime.UtcNow.AddHours(7);
            var queryDate = atDate ?? localNow.Date;
            var queryTime = atTime ?? localNow.TimeOfDay;

            var response = await _chairService.GetChairStatusBySalonAsync(salonId, queryDate, queryTime);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }


        /// <summary>
        /// Lấy chi tiết thông tin một ghế.
        /// </summary>
        [HttpGet("api/chairs/{id}")]
        [ProducesResponseType(typeof(ApiResult<ChairResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _chairService.GetChairByIdAsync(id);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Tạo mới một ghế.
        /// </summary>
        [HttpPost("api/chairs")]
        [ProducesResponseType(typeof(ApiResult<ChairResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ChairCreateRequest request)
        {
            var response = await _chairService.CreateChairAsync(request);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật thông tin một ghế.
        /// </summary>
        [HttpPut("api/chairs/{id}")]
        [ProducesResponseType(typeof(ApiResult<ChairResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChairUpdateRequest request)
        {
            var response = await _chairService.UpdateChairAsync(id, request);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Xóa một ghế.
        /// </summary>
        [HttpDelete("api/chairs/{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _chairService.DeleteChairAsync(id);
            if (!response.IsSucceeded) return BadRequest(response);
            return Ok(response);
        }
    }
}
