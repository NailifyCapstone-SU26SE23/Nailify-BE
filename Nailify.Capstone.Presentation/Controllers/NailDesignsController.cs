using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailDesignRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý mẫu nail.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NailDesignsController : ControllerBase
    {
        private readonly INailDesignService _nailDesignService;

        public NailDesignsController(INailDesignService nailDesignService)
        {
            _nailDesignService = nailDesignService;
        }

        /// <summary>
        /// Lấy danh sách tất cả mẫu nail.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<NailDesignDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _nailDesignService.GetAllNailDesignsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách mẫu nail phân trang.
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResult<PagedList<NailDesignDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _nailDesignService.GetPagedNailDesignsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết mẫu nail theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nailDesignService.GetNailDesignByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo mẫu nail mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] NailDesignCreateRequest request)
        {
            var result = await _nailDesignService.CreateNailDesignAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật mẫu nail.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<NailDesignDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] NailDesignUpdateRequest request)
        {
            var result = await _nailDesignService.UpdateNailDesignAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Xóa mẫu nail bằng cách chuyển trạng thái sang InActive.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _nailDesignService.DeleteNailDesignAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy mẫu nail theo danh mục.
        /// </summary>
        [HttpGet("by-category/{categoryId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailDesignDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _nailDesignService.GetNailDesignsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}
