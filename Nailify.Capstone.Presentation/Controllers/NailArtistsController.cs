using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NailArtistsController : ControllerBase
    {
        private readonly INailArtistService _artistService;

        public NailArtistsController(INailArtistService artistService)
        {
            _artistService = artistService;
        }

        /// <summary>
        /// Lấy danh sách thợ làm móng theo chi nhánh Salon
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailArtistResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? salonId = null, [FromQuery] ActiveStatusFilter? status = null, [FromQuery] string? orderBy = null)
        {
            var statusStr = (status == null || status == ActiveStatusFilter.All) ? null : status.ToString();
            var response = await _artistService.GetPagedNailArtistsAsync(pageNumber, pageSize, salonId, statusStr, orderBy);
            return Ok(response);
        }

        /// <summary>
        /// Lấy chi tiết thông tin một thợ làm móng (kèm hồ sơ tài khoản cá nhân).
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailArtistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _artistService.GetNailArtistByIdAsync(id);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

 

        /// <summary>
        /// Cập nhật thông tin thợ làm móng (đổi chi nhánh Salon làm việc hoặc trạng thái).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailArtistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] NailArtistUpdateRequest request)
        {
            var response = await _artistService.UpdateNailArtistAsync(id, request);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật từng phần thông tin thợ làm móng.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailArtistResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(Guid id, [FromBody] NailArtistPatchRequest request)
        {
            var response = await _artistService.PatchNailArtistAsync(id, request);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Xóa thợ làm móng
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _artistService.DeleteNailArtistAsync(id);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }
    }
}
