using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.TransactionResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : BaseApiController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<TransactionResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] TransactionStatus? status = null,
            [FromQuery] Guid? salonId = null)
        {
            return Ok(await _transactionService.GetPagedAsync(
                pageNumber,
                pageSize,
                startDate,
                endDate,
                status,
                salonId));
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResult<PagedList<TransactionResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMine(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] TransactionStatus? status = null)
        {
            return Ok(await _transactionService.GetMyPagedAsync(
                GetCurrentUserId(),
                pageNumber,
                pageSize,
                startDate,
                endDate,
                status));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResult<TransactionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _transactionService.GetByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpGet("booking/{bookingId:guid}")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<TransactionResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByBookingId(Guid bookingId)
        {
            return Ok(await _transactionService.GetByBookingIdAsync(bookingId));
        }
    }
}
