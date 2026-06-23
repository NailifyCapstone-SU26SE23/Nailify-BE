using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.CustomerNailRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.CustomerNailRequestResponseDTO;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.Service;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API quản lý yêu cầu duyệt mẫu nail của khách hàng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerNailRequestsController : BaseApiController
    {
        private readonly ICustomerNailRequestsService _customerNailRequestsService;

        public CustomerNailRequestsController(
            ICustomerNailRequestsService customerNailRequestsService)
        {
            _customerNailRequestsService = customerNailRequestsService;
        }

        /// <summary>
        /// Lấy danh sách yêu cầu duyệt mẫu móng (có phân trang và lọc theo Salon, Khách hàng, Thợ nail, Trạng thái).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<CustomerNailRequestResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? salonId = null,
            [FromQuery] CustomerNailStatus? status = null,
            [FromQuery] Guid? customerId = null,
            [FromQuery] Guid? approvedArtistId = null)
        {
            var result = await _customerNailRequestsService.GetPagedCustomerNailRequestsAsync(
                pageNumber, pageSize, salonId, status, customerId, approvedArtistId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết yêu cầu duyệt mẫu móng theo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerNailRequestResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            var result = await _customerNailRequestsService.GetCustomerNailRequestByIdAsync(id);
            return result.IsSucceeded ? Ok(result) : NotFound(result);
        }
    }
}
