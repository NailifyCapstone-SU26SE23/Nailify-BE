using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailCategoryRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NailCategoriesController : ControllerBase
    {
        private readonly INailCategoryService _nailCategoryService;

        public NailCategoriesController(INailCategoryService nailCategoryService)
        {
            _nailCategoryService = nailCategoryService;
        }

        [HttpGet("nail-design/{nailDesignId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailCategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNailDesignId(int nailDesignId)
        {
            var result = await _nailCategoryService.GetByNailDesignIdAsync(nailDesignId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpPost("nail-design/{nailDesignId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailCategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(int nailDesignId, [FromBody] List<NailCategoryRequest> request)
        {
            var result = await _nailCategoryService.AssignCategoriesToNailDesignAsync(nailDesignId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpPut("nail-design/{nailDesignId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailCategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int nailDesignId, [FromBody] List<NailCategoryRequest> request)
        {
            var result = await _nailCategoryService.AssignCategoriesToNailDesignAsync(nailDesignId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("nail-design/{nailDesignId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailCategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int nailDesignId)
        {
            var result = await _nailCategoryService.DeleteByNailDesignIdAsync(nailDesignId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
