using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.DTOs.RequestDTOs.LoyaltyTransactionRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
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
    }
}
