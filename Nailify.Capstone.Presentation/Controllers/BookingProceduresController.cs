using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý tiến trình thực hiện quy trình làm móng của khách hàng (BookingProcedure).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookingProceduresController : BaseApiController
    {
        private readonly IBookingProcedureService _bookingProcedureService;

        public BookingProceduresController(IBookingProcedureService bookingProcedureService)
        {
            _bookingProcedureService = bookingProcedureService;
        }

        /// <summary>
        /// Lấy danh sách các bước quy trình thực tế của một mục đặt lịch (BookingItem).
        /// </summary>
        [HttpGet("booking-item/{bookingItemId}")]
        [ProducesResponseType(typeof(ApiResult<List<BookingProcedureResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByBookingItemId(Guid bookingItemId)
        {
            var result = await _bookingProcedureService.GetProceduresByBookingItemIdAsync(bookingItemId);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật trạng thái thực hiện của một bước quy trình làm móng cho Booking.
        /// </summary>
        /// <param name="bookingProcedureId">ID của bước quy trình thực tế.</param>
        /// <param name="artistId">ID của thợ nail thực hiện.</param>
        /// <param name="status">Trạng thái mới (Pending, InProgress, Completed, Skipped).</param>
        [HttpPut("{bookingProcedureId}/status")]
        [ProducesResponseType(typeof(ApiResult<BookingProcedureResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(
            Guid bookingProcedureId,
            [FromQuery] Guid artistId,
            [FromQuery] BookingProcedureStatus status)
        {
            var result = await _bookingProcedureService.UpdateProcedureStatusAsync(bookingProcedureId, artistId, status);
            if (!result.IsSucceeded)
            {
                if (result.Message.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Thợ nail tự nhận (claim) thực hiện một bước quy trình làm móng đang chờ của khách hàng.
        /// </summary>
        /// <param name="procedureId">Mã định danh (ID) của bước quy trình thực tế (BookingProcedureId).</param>
        /// <returns>Thông tin bước quy trình sau khi đã được nhận.</returns>
        [Authorize(Roles = "Staff_Artist,Receptionist,Manager")]
        [HttpPost("procedures/{procedureId}/claim")]
        [ProducesResponseType(typeof(ApiResult<BookingProcedureResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClaimProcedure(Guid procedureId)
        {
            // Lấy AccountId từ JWT Token của thợ đang đăng nhập
            var accountId = GetCurrentUserId();

            var result = await _bookingProcedureService.ClaimProcedureStepAsync(procedureId, accountId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách thợ nail tại salon để chỉ định, kèm theo trạng thái rảnh (IsFree) và đủ điều kiện kỹ năng (IsQualified).
        /// </summary>
        [HttpGet("{bookingProcedureId}/available-artists")]
        [ProducesResponseType(typeof(ApiResult<List<IdleArtistResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvailableArtists(Guid bookingProcedureId)
        {
            var result = await _bookingProcedureService.GetAvailableArtistsForProcedureAsync(bookingProcedureId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
