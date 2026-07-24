using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ProcedureRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.ProcedureResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý quy trình làm móng chuẩn (Procedure).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProceduresController : ControllerBase
    {
        private readonly IProcedureService _procedureService;

        public ProceduresController(IProcedureService procedureService)
        {
            _procedureService = procedureService;
        }

        /// <summary>
        /// Lấy danh sách quy trình làm móng phân trang.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<ProcedureResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestParameters parameters)
        {
            var result = await _procedureService.GetAllProceduresAsync(parameters);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết quy trình làm móng chuẩn theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<ProcedureResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _procedureService.GetProcedureByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Tạo một bước quy trình làm móng mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<ProcedureResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProcedureRequestDTO request)
        {
            var result = await _procedureService.CreateProcedureAsync(request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin quy trình làm móng chuẩn.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<ProcedureResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProcedureRequestDTO request)
        {
            var result = await _procedureService.UpdateProcedureAsync(id, request);
            if (!result.IsSucceeded)
            {
                if (result.Message.Contains("Không tìm thấy"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Xóa một bước quy trình làm móng chuẩn.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _procedureService.DeleteProcedureAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách quy trình đã cấu hình của một mẫu móng.
        /// </summary>
        [HttpGet("variant/{nailVariantId}")]
        [ProducesResponseType(typeof(ApiResult<List<ProcedureResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByVariantId(int nailVariantId)
        {
            var result = await _procedureService.GetProceduresByVariantIdAsync(nailVariantId);
            return Ok(result);
        }

        [HttpGet("customer-nail/{customerNailId}")]
        [ProducesResponseType(typeof(ApiResult<List<NailProcedureResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCustomerNailId(int customerNailId)
        {
            var result = await _procedureService.GetNailProceduresByCustomerNailIdAsync(customerNailId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        [HttpPut("customer-nail/procedure/{nailProcedureId}")]
        [ProducesResponseType(typeof(ApiResult<NailProcedureResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomerNailProcedure(Guid nailProcedureId, [FromBody] CustomerNailProcedureRequestDTO request)
        {
            var result = await _procedureService.UpdateCustomerNailProcedureAsync(nailProcedureId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("customer-nail/procedure/{nailProcedureId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomerNailProcedure(Guid nailProcedureId)
        {
            var result = await _procedureService.DeleteCustomerNailProcedureAsync(nailProcedureId);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Cấu hình/Gán quy trình làm móng cho một mẫu móng.
        /// </summary>
        [HttpPost("assign/{nailVariantId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignProceduresToVariant(int nailVariantId, [FromBody] List<AssignProcedureRequestDTO> request)
        {
            var result = await _procedureService.AssignProceduresToVariantAsync(nailVariantId, request);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("assign/customer-nail/{customerNailId}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignProceduresToCustomerNail(int customerNailId, [FromBody] List<CustomerNailProcedureRequestDTO> request)
        {
            var result = await _procedureService.AssignProceduresToCustomerNailAsync(customerNailId, request);
            return result.IsSucceeded ? Ok(result) : BadRequest(result);
        }
    }
}
