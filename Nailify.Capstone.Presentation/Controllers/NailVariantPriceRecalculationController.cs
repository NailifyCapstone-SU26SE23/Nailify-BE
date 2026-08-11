using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Presentation.Middlewares;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/nail-variant-prices")]
    [HasRole("Admin")]
    public class NailVariantPriceRecalculationController : ControllerBase
    {
        private readonly INailVariantPriceRecalculationService _recalculationService;

        public NailVariantPriceRecalculationController(INailVariantPriceRecalculationService recalculationService)
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
    }
}
