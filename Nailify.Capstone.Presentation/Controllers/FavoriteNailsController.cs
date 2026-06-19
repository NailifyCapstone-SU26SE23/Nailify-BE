using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.FavoriteNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FavoriteNailsController : BaseApiController
    {
        private readonly IFavoriteNailService _favoriteNailService;

        public FavoriteNailsController(IFavoriteNailService favoriteNailService)
        {
            _favoriteNailService = favoriteNailService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<FavoriteNailDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
            => Ok(await _favoriteNailService.GetPagedAsync(GetCurrentUserId(), pageNumber, pageSize));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _favoriteNailService.GetByIdAsync(GetCurrentUserId(), id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FavoriteNailRequest request)
        {
            var result = await _favoriteNailService.CreateAsync(GetCurrentUserId(), request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FavoriteNailRequest request)
        {
            var result = await _favoriteNailService.UpdateAsync(GetCurrentUserId(), id, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _favoriteNailService.DeleteAsync(GetCurrentUserId(), id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
