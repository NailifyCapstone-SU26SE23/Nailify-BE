using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.Dashboard;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdminDashboard(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            return Ok(await _dashboardService.GetAdminDashboardDataAsync(startDate, endDate));
        }

        [HttpGet("nail-artist/{artistId:guid}")]
        [Authorize(Roles = "Staff_Artist,Manager,Admin")]
        [ProducesResponseType(typeof(NailArtistDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNailArtistDashboard(
            Guid artistId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            return Ok(await _dashboardService.GetNailArtistDashboardDataAsync(artistId, startDate, endDate));
        }

        [HttpGet("receptionist/{salonId:guid}")]
        [Authorize(Roles = "Receptionist,Manager,Admin")]
        [ProducesResponseType(typeof(ReceptionistDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReceptionistDashboard(
            Guid salonId,
            [FromQuery] DateTime? date = null)
        {
            return Ok(await _dashboardService.GetReceptionistDashboardDataAsync(salonId, date));
        }

        [HttpGet("salon/{salonId:guid}")]
        [Authorize(Roles = "Manager,Admin")]
        [ProducesResponseType(typeof(SalonManagerDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalonDashboard(
            Guid salonId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            return Ok(await _dashboardService.GetSalonDashboardDataAsync(salonId, startDate, endDate));
        }
    }
}
