using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionsController : BaseApiController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
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

        [HttpGet("me")]
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
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _transactionService.GetByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpGet("booking/{bookingId:guid}")]
        public async Task<IActionResult> GetByBookingId(Guid bookingId)
        {
            return Ok(await _transactionService.GetByBookingIdAsync(bookingId));
        }
    }
}
