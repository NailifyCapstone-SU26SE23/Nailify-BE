using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.DTOs.PaymentDTOs;
using Nailify.Capstone.Infrastructure.Service;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PayOSService _paymentService;

        public PaymentsController(PayOSService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create/{bookingId}")]
        public async Task<IActionResult> CreatePaymentLink(Guid bookingId)
        {
            var result = await _paymentService.CreatePaymentLinkAsync(bookingId);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                result.Message,
                data = result.Payment
            });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymentWebhookDto webhookData)
        {
            var result = await _paymentService.HandlePaymentWebhookAsync(webhookData);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        [HttpGet("status/{orderCode}")]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            var result = await _paymentService.GetPaymentStatusAsync(orderCode);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new
            {
                result.Message,
                status = result.Status
            });
        }

        [HttpPost("cancel/{orderCode}")]
        public async Task<IActionResult> CancelPaymentLink(long orderCode)
        {
            var result = await _paymentService.CancelPaymentLinkAsync(orderCode);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}