using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ServiceRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ServiceResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;


namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý danh mục dịch vụ gốc của tiệm.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        public ServicesController(
            IServicesService servicesService)
        {
            _servicesService = servicesService;
        }
        /// <summary>
        /// Lấy danh sách dịch vụ gốc phân trang, hỗ trợ tìm theo tên.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<ServiceResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null)
        {
            var result = await _servicesService.GetPagedServicesAsync(pageNumber, pageSize, name);
            return Ok(result);
        }
        /// <summary>
        /// Lấy thông tin chi tiết của dịch vụ theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ServiceResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _servicesService.GetServiceByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Thêm mới một dịch vụ gốc vào hệ thống.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<ServiceResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ServiceCreateRequestDTO request)
        {
            var result = await _servicesService.CreateServiceAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật thông tin dịch vụ gốc.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<ServiceResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceUpdateRequestDTO request)
        { 
            var result = await _servicesService.UpdateServiceAsync(id, request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Xóa dịch vụ gốc.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _servicesService.DeleteServiceAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
