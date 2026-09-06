using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class LoyaltyTransactionsController : BaseApiController
    {
        private readonly ILoyaltyTransactionService _service;
        public LoyaltyTransactionsController(ILoyaltyTransactionService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? userId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _service.GetPagedAsync(pageNumber, pageSize, userId));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            return Ok(await _service.GetPagedAsync(pageNumber, pageSize, GetCurrentUserId()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSucceeded) return NotFound(result);
            if (!User.IsInRole(nameof(UserRole.Admin)) && result.Data.CustomerId != GetCurrentUserId()) return Forbid();
            return Ok(result);
        }
        /// <summary>
        /// Lấy tổng quan Ví điểm và thông tin Hạng thành viên của khách hàng đang đăng nhập.
        /// </summary>
        [HttpGet("my-wallet-summary")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<WalletSummaryDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWalletSummary()
        {
            var userId = GetCurrentUserId();
            
            var result = await _service.GetWalletSummaryAsync(userId);
            return Ok(result);
        }
        /// <summary>
        /// Hoàn điểm trực tiếp vào ví điểm của khách hàng (Dành cho Admin/Salon khi xử lý hoàn đơn hoặc đền bù).
        /// </summary>
        [HttpPost("refund-points")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(typeof(ApiResult<LoyaltyTransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<LoyaltyTransactionDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefundPointsToWallet([FromBody] RefundPointsRequest request)
        {
            var result = await _service.RefundPointsToWalletAsync(
                request.CustomerId,
                request.BookingId,
                request.PointsToRefund,
                request.Reason);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
