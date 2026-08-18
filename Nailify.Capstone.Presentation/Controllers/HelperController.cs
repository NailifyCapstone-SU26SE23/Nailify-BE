using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Presentation.Middlewares;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/recalculation")]
    [HasRole("Admin")]
    public class HelperController : ControllerBase
    {
        private readonly IRecalculationService _recalculationService;

        public HelperController(IRecalculationService recalculationService)
        {
            _recalculationService = recalculationService;
        }

        [HttpPost("recalculate")]
        [ProducesResponseType(typeof(ApiResult<NailVariantPriceRecalculationResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RecalculateAll()
        {
            var result = await _recalculationService.RecalculateAllAsync();
            return Ok(result);
        }

        [HttpPost("customer-nails/recalculate")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailPriceRecalculationResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RecalculateAllCustomerNails()
        {
            var result = await _recalculationService.RecalculateAllCustomerNailsAsync();
            return Ok(result);
        }

        [HttpPost("process-all-completed")]
        public async Task<IActionResult> ProcessAllCompletedBookings()
        {
            var result = await _recalculationService.ProcessAllCompletedBookingsAsync();

            return Ok();
        }
    }
}
