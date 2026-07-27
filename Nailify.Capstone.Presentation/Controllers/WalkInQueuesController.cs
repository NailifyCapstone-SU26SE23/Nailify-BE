using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalkInQueueResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalkInQueuesController : BaseApiController
    {
        private readonly IWalkInQueueService _queueService;

        public WalkInQueuesController(IWalkInQueueService queueService)
        {
            _queueService = queueService;
        }

        /// <summary>
        /// Đăng ký thêm khách hàng vãng lai hoặc khách đặt lịch trễ (Late Arrival) vào hàng chờ tại sảnh.
        /// </summary>
        /// <param name="request">Thông tin đăng ký hàng chờ vãng lai.</param>
        /// <returns>Bản ghi hàng chờ vừa được tạo.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToQueue([FromBody] AddToQueueRequestDTO request)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.AddToQueueAsync(actorId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy danh sách tất cả các lượt chờ hôm nay của một chi nhánh Salon (dành cho Lễ tân theo dõi).
        /// </summary>
        /// <param name="salonId">Mã định danh (ID) của chi nhánh Salon.</param>
        /// <returns>Danh sách hàng chờ trong ngày hôm nay.</returns>
        [HttpGet("salon/{salonId}/today")]
        [ProducesResponseType(typeof(ApiResult<List<WalkInQueueResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTodayQueue(Guid salonId)
        {
            var result = await _queueService.GetTodayQueueAsync(salonId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lễ tân gọi số thứ tự khách hàng lên quầy để bắt đầu tư vấn dịch vụ và gán thợ.
        /// </summary>
        /// <param name="id">Mã định danh (ID) của lượt chờ cần gọi.</param>
        /// <returns>Thông tin hàng chờ sau khi chuyển sang trạng thái Called.</returns>
        [HttpPost("{id}/call")]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Call(Guid id)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.CallQueueAsync(id, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lễ tân gán thợ làm móng đang rảnh cho lượt chờ của khách.
        /// </summary>
        /// <param name="id">Mã định danh (ID) của lượt chờ cần gán thợ.</param>
        /// <param name="request">Thông tin thợ làm móng cần gán.</param>
        /// <returns>Thông tin hàng chờ sau khi đã gán thợ thành công.</returns>
        [HttpPost("{id}/assign-artist")]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignArtist(Guid id, [FromBody] AssignQueueArtistRequestDTO request)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.AssignArtistAsync(id, request, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Đánh dấu hoàn thành lượt xếp hàng (sau khi đã tạo thành công Booking cho khách).
        /// </summary>
        /// <param name="id">Mã định danh (ID) của lượt chờ cần hoàn thành.</param>
        /// <returns>Thông tin hàng chờ sau khi chuyển sang trạng thái Completed.</returns>
        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Complete(Guid id)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.CompleteQueueEntryAsync(id, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Đánh dấu khách hàng vắng mặt hoặc tự ý bỏ về (Rời hàng chờ).
        /// </summary>
        /// <param name="id">Mã định danh (ID) của lượt chờ cần đánh dấu.</param>
        /// <returns>Thông tin hàng chờ sau khi chuyển sang trạng thái Left.</returns>
        [HttpPost("{id}/mark-left")]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkLeft(Guid id)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.MarkLeftAsync(id, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Lễ tân đẩy một lượt chờ lên đầu hàng chờ sảnh của thợ đó để được phục vụ trước.
        /// </summary>
        [HttpPost("{id}/prioritize")]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Prioritize(Guid id)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.PrioritizeQueueEntryAsync(id, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tự động khởi tạo Tài khoản khách hàng (nếu là khách vãng lai) và chuyển lượt chờ Walk-in sang đơn Booking chính thức.
        /// </summary>
        [HttpPost("{id}/convert-to-booking")]
        [ProducesResponseType(typeof(ApiResult<BookingResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConvertToBooking(Guid id)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.ConvertWalkInToBookingAsync(id, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Lễ tân phân ghế cho khách đang đợi trong hàng chờ Walk-in.
        /// Chỉ được phân khi lượt chờ đang ở trạng thái Waiting hoặc Called.
        /// </summary>
        /// <param name="id">ID của lượt chờ cần phân ghế.</param>
        /// <param name="request">Chứa ChairId cần gán.</param>
        /// <returns>Thông tin hàng chờ sau khi gán ghế thành công (bao gồm ChairId và ChairName).</returns>
        [HttpPost("{id}/assign-chair")]
        [ProducesResponseType(typeof(ApiResult<WalkInQueueResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignChair(Guid id, [FromBody] AssignQueueChairRequestDTO request)
        {
            var actorId = GetCurrentUserId();
            var result = await _queueService.AssignChairToQueueAsync(id, request, actorId);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

    }
}
