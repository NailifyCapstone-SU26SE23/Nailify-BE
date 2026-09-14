using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalletRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalletResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý ví tiền điện tử và ví điểm thưởng (Dual-Wallet System).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletsController : BaseApiController
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Lấy thông tin tổng quan ví tiền cá nhân và điểm thưởng của khách hàng đang đăng nhập.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ApiResult<CustomerWalletSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWalletSummary()
        {
            var customerId = GetCurrentUserId();
            var result = await _walletService.GetWalletSummaryAsync(customerId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin điểm thưởng của khách hàng theo mã ví (Dành cho Admin).
        /// </summary>
        /// <param name="walletId">Mã ví của khách hàng.</param>
        [HttpGet("getById/{walletId:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResult<WalletSummaryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<WalletSummaryDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid walletId)
        {
            var result = await _walletService.GetWalletSummaryByIdAsync(walletId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yêu cầu tạo link nạp tiền vào ví cá nhân qua cổng thanh toán online (PayOS).
        /// </summary>
        /// <param name="amount">Số tiền nạp tối thiểu là 10,000 VNĐ.</param>
        [HttpPost("deposit")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestDeposit([FromBody] decimal amount)
        {
            var customerId = GetCurrentUserId();
            var result = await _walletService.RequestDepositAsync(customerId, amount);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Gửi yêu cầu rút tiền từ ví cá nhân về tài khoản ngân hàng (Tiền sẽ bị đóng băng cho đến khi Admin xử lý).
        /// </summary>
        /// <param name="request">Thông tin ngân hàng nhận và số tiền muốn rút.</param>
        [HttpPost("withdraw")]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestWithdrawal([FromBody] CreateWithdrawalRequestDto request)
        {
            var customerId = GetCurrentUserId();
            var result = await _walletService.RequestWithdrawalAsync(customerId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Quy đổi tiền trong ví cá nhân sang điểm thưởng loyalty theo cơ chế 1 chiều (100 VNĐ = 1 điểm).
        /// </summary>
        /// <param name="request">Số tiền nạp quy đổi (Phải là bội số của 10,000 VNĐ).</param>
        [HttpPost("convert-points")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConvertMoneyToPoints([FromBody] ConvertMoneyToPointsRequestDto request)
        {
            var customerId = GetCurrentUserId();
            var result = await _walletService.ConvertMoneyToPointsAsync(customerId, request.MoneyAmount);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xem lịch sử giao dịch chi tiết của ví cá nhân (Nạp tiền, rút tiền, thanh toán booking, quy đổi điểm).
        /// </summary>
        /// <param name="pageNumber">Số trang (Mặc định: 1).</param>
        /// <param name="pageSize">Kích thước trang (Mặc định: 10).</param>
        [HttpGet("transactions")]
        [ProducesResponseType(typeof(ApiResult<PagedList<WalletTransactionResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var customerId = GetCurrentUserId();
            var result = await _walletService.GetTransactionHistoryAsync(customerId, pageNumber, pageSize);
            return Ok(result);
        }


        /// <summary>
        /// Lấy danh sách lịch sử giao dịch ví tiền toàn hệ thống kèm các bộ lọc nâng cao (Dành cho Admin).
        /// </summary>
        /// <param name="type">Loại giao dịch (Deposit, Withdraw, BookingPayment, ConvertToPoints,...).</param>
        /// <param name="status">Trạng thái giao dịch (Pending, Completed, Failed, Cancelled).</param>
        /// <param name="fromDate">Từ ngày.</param>
        /// <param name="toDate">Đến ngày.</param>
        /// <param name="pageNumber">Số trang (Mặc định: 1).</param>
        /// <param name="pageSize">Kích thước trang (Mặc định: 10).</param>
        [HttpGet("admin/transactions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResult<PagedList<WalletTransactionResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemTransactionHistory(
            [FromQuery] WalletTransactionType? type,
            [FromQuery] WalletTransactionStatus? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _walletService.GetSystemTransactionHistoryAsync(type, status, fromDate, toDate, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một giao dịch ví theo mã giao dịch.
        /// </summary>
        /// <param name="walletTransactionId">Mã giao dịch ví (WalletTransactionId).</param>
        [HttpGet("transactions/{walletTransactionId:guid}")]
        [ProducesResponseType(typeof(ApiResult<WalletTransactionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<WalletTransactionResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWalletTransactionById(Guid walletTransactionId)
        {
            var result = await _walletService.GetWalletTransactionByIdAsync(walletTransactionId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Lấy danh sách tất cả các yêu cầu rút tiền toàn hệ thống kèm bộ lọc trạng thái (Dành cho Admin).
        /// </summary>
        /// <param name="status">Trạng thái rút tiền (Pending, Approved, Rejected).</param>
        /// <param name="pageNumber">Số trang (Mặc định: 1).</param>
        /// <param name="pageSize">Kích thước trang (Mặc định: 10).</param>
        [HttpGet("admin/withdrawals")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResult<PagedList<WithdrawalRequestResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWithdrawals([FromQuery] WithdrawalStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _walletService.GetAllWithdrawalsAsync(status, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một yêu cầu rút tiền theo mã yêu cầu.
        /// </summary>
        /// <param name="requestId">Mã yêu cầu rút tiền (WithdrawalRequestId).</param>
        [HttpGet("withdrawals/{requestId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWithdrawalById(Guid requestId)
        {
            var result = await _walletService.GetWithdrawalByIdAsync(requestId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Admin duyệt yêu cầu rút tiền của khách hàng và xác nhận đã chuyển khoản thành công.
        /// </summary>
        /// <param name="requestId">Mã yêu cầu rút tiền (WithdrawalRequestId).</param>
        /// <param name="dto">Ghi chú và mã giao dịch ngân hàng đối soát.</param>
        [HttpPost("withdrawals/{requestId:guid}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApproveWithdrawal(Guid requestId, [FromBody] ApproveWithdrawalDto dto)
        {
            var adminId = GetCurrentUserId();
            var result = await _walletService.ApproveWithdrawalAsync(adminId, requestId, dto);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Admin từ chối yêu cầu rút tiền của khách hàng và hoàn trả tiền đóng băng về số dư khả dụng.
        /// </summary>
        /// <param name="requestId">Mã yêu cầu rút tiền (WithdrawalRequestId).</param>
        /// <param name="dto">Ghi chú lý do từ chối.</param>
        [HttpPost("withdrawals/{requestId:guid}/reject")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<WithdrawalRequestResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RejectWithdrawal(Guid requestId, [FromBody] RejectWithdrawalDto dto)
        {
            var adminId = GetCurrentUserId();
            var result = await _walletService.RejectWithdrawalAsync(adminId, requestId, dto);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy thống kê tổng quan ví toàn hệ thống (Tổng số dư, tổng tiền đóng băng, tổng số lượt rút) (Dành cho Admin).
        /// </summary>
        [HttpGet("system-summary")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResult<SystemWalletSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemWalletSummary()
        {
            var result = await _walletService.GetSystemWalletSummaryAsync();
            return Ok(result);
        }
    }
}
