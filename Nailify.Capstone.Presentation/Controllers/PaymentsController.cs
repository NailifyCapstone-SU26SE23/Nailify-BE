using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Infrastructure.Service;
using BankAccountInfo = Nailify.Capstone.Infrastructure.Configuration.PayOS.BankAccountInfo;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PayOSService _paymentService;
        private readonly RefundService _refundService;

        public PaymentsController(PayOSService paymentService, RefundService refundService)
        {
            _paymentService = paymentService;
            _refundService = refundService;
        }

        [HttpPost("create/{bookingId}")]
        public async Task<IActionResult> CreatePaymentLink(Guid bookingId)
        {
            var result = await _paymentService.CreatePaymentLinkAsync(bookingId);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<PaymentResponseDto?>(result.Payment, result.Message));
        }

        [HttpPost("refund/{bookingId}")]
        public async Task<IActionResult> CreateRefundLink(Guid bookingId, [FromBody] CreateRefundLinkRequest request)
        {
            if (request?.BankInfo == null)
                return BadRequest(new ApiErrorResult<object>("Thong tin tai khoan ngan hang la bat buoc."));

            var result = await _refundService.CreateSinglePayoutByBookingAsync(
                bookingId,
                request.BankInfo,
                request.Reason);

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

        [HttpPost("webhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymentWebhookDto webhookData)
        {
            var result = await _paymentService.HandlePaymentWebhookAsync(webhookData);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<object?>(null, result.Message));
        }

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

        [HttpPost("cancel/{orderCode}")]
        public async Task<IActionResult> CancelPaymentLink(long orderCode)
        {
            var result = await _paymentService.CancelPaymentLinkAsync(orderCode);

            if (!result.Success)
                return BadRequest(new ApiErrorResult<object>(result.Message));

            return Ok(new ApiSuccessResult<object?>(null, result.Message));
        }
    }

    public class CreateRefundLinkRequest
    {
        public BankAccountInfo BankInfo { get; set; } = new();
        public string? Reason { get; set; }
    }
}
