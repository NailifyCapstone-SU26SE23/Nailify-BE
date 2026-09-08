using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System.Security.Claims;
using Nailify.Capstone.Infrastructure.Service;
using BankAccountInfo = Nailify.Capstone.Infrastructure.Configuration.PayOS.BankAccountInfo;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API xử lý thanh toán và hoàn tiền.
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PayOSService _paymentService;
        private readonly RefundService _refundService;
        private readonly IPromotionService _promotionService;

        public PaymentsController(
            PayOSService paymentService,
            RefundService refundService,
            IPromotionService promotionService)
        {
            _paymentService = paymentService;
            _refundService = refundService;
            _promotionService = promotionService;
        }

        /// <summary>
        /// Tạo link thanh toán cho lịch hẹn.
        /// </summary>
        [HttpPost("create/{bookingId}")]
        public async Task<IActionResult> CreatePaymentLink(Guid bookingId)
        {
            var result = await _paymentService.CreatePaymentLinkAsync(bookingId);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<PaymentResponseDto?>(result.Payment, result.Message));
        }

        /// <summary>
        /// Tạo link thanh toán từ yêu cầu đặt lịch.
        /// </summary>
        [HttpPost("create-for-request")]
        public async Task<IActionResult> CreatePaymentLinkFromRequest([FromBody] CreateBookingRequestDTO request)
        {
            var customerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Unauthorized(new ApiErrorResult<object>("Không tìm thấy thông tin tài khoản."));
            }

            var result = await _paymentService.CreatePaymentLinkForBookingRequestAsync(customerId, request);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<PaymentResponseDto?>(result.Payment, result.Message));
        }

        /// <summary>
        /// Tạo yêu cầu hoàn tiền cho lịch hẹn.
        /// </summary>
        [HttpPost("refund/{bookingId}")]
        public async Task<IActionResult> CreateRefundLink(Guid bookingId, [FromBody] BankAccountInfo request)
        {
            if (request == null)
                return BadRequest(new ApiErrorResult<object>("Vui lòng nhập thông tin tài khoản ngân hàng."));

            var result = await _refundService.CreateSinglePayoutByBookingAsync(
                bookingId,
                request);
            if (!result.Success)
            {
                return BadRequest(new
                {
                    IsSucceeded = false,
                    result.Message,
                    Data = (object?)null,
                    result.ErrorCode,
                    result.ErrorDescription
                });
            }

            return Ok(new ApiSuccessResult<object?>(result.Transaction, result.Message));
        }

        /// <summary>
        /// Hoàn toàn bộ tiền cọc cho lịch hẹn (khi không có nhân viên thay thế).
        /// </summary>
        [HttpPost("fullRefund/{bookingId}")]
        public async Task<IActionResult> FullRefund(Guid bookingId, [FromBody] BankAccountInfo request)
        {
            if (request == null)
                return BadRequest(new ApiErrorResult<object>("Vui lòng nhập thông tin tài khoản ngân hàng."));

            var result = await _refundService.CreateSinglePayoutByBookingAsync(
                bookingId,
                request,
                "Hoàn toàn bộ tiền cọc do không có nhân viên thay thế.",
                forceFullRefund: true);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    IsSucceeded = false,
                    result.Message,
                    Data = (object?)null,
                    result.ErrorCode,
                    result.ErrorDescription
                });
            }

            var voucherResult = await _promotionService.AddVoucherForCancelledAsync(bookingId);

            return Ok(new ApiSuccessResult<object?>(new
            {
                Refund = result.Transaction,
                Voucher = voucherResult.IsSucceeded ? voucherResult.Data : null,
                VoucherMessage = voucherResult.Message
            }, result.Message));
        }

        /// <summary>
        /// Hoàn tiền khi lịch hẹn bị từ chối.
        /// </summary>
        [HttpPost("refund/reject/{bookingId}")]
        public async Task<IActionResult> RejectRefund(Guid bookingId, [FromBody] BankAccountInfo request)
        {
            if (request == null)
                return BadRequest(new ApiErrorResult<object>("Vui lòng nhập thông tin tài khoản ngân hàng."));

            var result = await _refundService.CreateSinglePayoutByBookingAsync(
                bookingId,
                request,
                "Hoàn toàn bộ tiền cọc do lịch hẹn bị từ chối.",
                forceFullRefund: true);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    IsSucceeded = false,
                    result.Message,
                    Data = (object?)null,
                    result.ErrorCode,
                    result.ErrorDescription
                });
            }

            return Ok(new ApiSuccessResult<object?>(result.Transaction, result.Message));
        }

        /// <summary>
        /// Webhook nhận thông báo kết quả thanh toán từ PayOS.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymentWebhookDto webhookData)
        {
            var result = await _paymentService.HandlePaymentWebhookAsync(webhookData);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<object?>(null, result.Message));
        }

        /// <summary>
        /// Kiểm tra trạng thái thanh toán theo mã đơn hàng.
        /// </summary>
        [HttpGet("status/{orderCode}")]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            var result = await _paymentService.GetPaymentStatusAsync(orderCode);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<object>(new
            {
                Status = result.Status
            }, result.Message));
        }

        /// <summary>
        /// Hủy link thanh toán.
        /// </summary>
        [HttpPost("cancel/{orderCode}")]
        public async Task<IActionResult> CancelPaymentLink(long orderCode)
        {
            var result = await _paymentService.CancelPaymentLinkAsync(orderCode);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<object?>(null, result.Message));
        }
    }
}