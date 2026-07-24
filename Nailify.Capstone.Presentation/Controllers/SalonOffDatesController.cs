using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalonOffDatesController : ControllerBase
    {
        private readonly ISalonOffDateService _offDateService;
        public SalonOffDatesController(ISalonOffDateService offDateService)
        {
            _offDateService = offDateService;
        }
        /// <summary>
        /// Đăng ký ngày nghỉ hoặc khoảng ngày nghỉ cho chi nhánh.
        /// </summary>
        [HttpPost("salons/{salonId}")]
        [ProducesResponseType(typeof(ApiResult<SalonOffDateResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddOffDate(Guid salonId, [FromBody] CreateSalonOffDateRequestDTO request)
        {
            var result = await _offDateService.AddSalonOffDateAsync(salonId, request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách tất cả các ngày nghỉ của chi nhánh.
        /// </summary>
        [HttpGet("salons/{salonId}")]
        [ProducesResponseType(typeof(ApiResult<List<SalonOffDateResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOffDates(Guid salonId)
        {
            var result = await _offDateService.GetSalonOffDatesAsync(salonId);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật ngày nghỉ hoặc khoảng ngày nghỉ của chi nhánh.
        /// </summary>
        [HttpPut("{offDateId}")]
        [ProducesResponseType(typeof(ApiResult<SalonOffDateResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOffDate(Guid offDateId, [FromBody] UpdateSalonOffDateRequestDTO request)
        {
            var result = await _offDateService.UpdateSalonOffDateAsync(offDateId, request);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Xóa ngày nghỉ của chi nhánh.
        /// </summary>
        [HttpDelete("{offDateId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteOffDate(Guid offDateId)
        {
            var result = await _offDateService.DeleteSalonOffDateAsync(offDateId);
            if (!result.IsSucceeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
