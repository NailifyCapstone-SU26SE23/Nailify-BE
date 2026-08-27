using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.SalonRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.SalonResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalonsController : ControllerBase
    {
        private readonly ISalonService _salonService;
        private readonly CloudinaryService _cloudinaryService;

        public SalonsController(ISalonService salonService, CloudinaryService cloudinaryService)
        {
            _salonService = salonService;
            _cloudinaryService = cloudinaryService;
        }
        /// <summary>
        /// Lấy danh sách chi nhánh phân trang động.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<SalonResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] SalonRequestParameters parameters)
        {
            var response = await _salonService.GetPagedSalonsAsync(parameters);
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách chi nhánh (Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        [ProducesResponseType(typeof(ApiResult<PagedList<SalonResponseDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedAdmin([FromQuery] SalonRequestParameters parameters)
        {
            var response = await _salonService.GetPagedSalonsAdminAsync(parameters);
            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin chi tiết một chi nhánh.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<SalonResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _salonService.GetSalonByIdAsync(id);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }
        /// <summary>
        /// Tạo mới chi nhánh.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<SalonResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] SalonCreateRequest request, IFormFile? image)
        {
            try
            {
                string? imageUrl = null;
                if (image != null && image.Length > 0)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image);
                }

                var response = await _salonService.CreateSalonAsync(request, imageUrl);
                if (!response.IsSucceeded) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResult<object>(false, $"Tạo mới chi nhánh thất bại khi tải ảnh: {ex.Message}"));
            }
        }
        /// <summary>
        /// Cập nhật thông tin chi nhánh.
        /// </summary>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<SalonResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromForm] SalonUpdateRequest request, IFormFile? image)
        {
            try
            {
                string? imageUrl = null;
                if (image != null && image.Length > 0)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image);
                }

                var response = await _salonService.UpdateSalonAsync(id, request, imageUrl);
                if (!response.IsSucceeded) return NotFound(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResult<object>(false, $"Cập nhật chi nhánh thất bại khi tải ảnh: {ex.Message}"));
            }
        }
        /// <summary>
        /// Cập nhật một phần thông tin chi nhánh.
        /// </summary>
        [HttpPatch("{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<SalonResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(Guid id, [FromForm] SalonPatchRequest request, IFormFile? image)
        {
            try
            {
                string? imageUrl = null;
                if (image != null && image.Length > 0)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(image);
                }

                var response = await _salonService.PatchSalonAsync(id, request, imageUrl);
                if (!response.IsSucceeded) return NotFound(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResult<object>(false, $"Cập nhật một phần chi nhánh thất bại khi tải ảnh: {ex.Message}"));
            }
        }
        /// <summary>
        /// Xóa chi nhánh.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _salonService.DeleteSalonAsync(id);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }
        /// <summary>
        /// Cập nhật giờ hoạt động của Salon.
        /// </summary>
        [HttpPut("{id}/operating-hours")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOperatingHours(Guid id, [FromBody] List<SalonOperatingHourUpdateRequest> request)
        {
            var response = await _salonService.UpdateOperatingHoursAsync(id, request);
            if (!response.IsSucceeded) return NotFound(response);
            return Ok(response);
        }

        /// <summary>
        /// Tải lên ảnh cho chi nhánh.
        /// </summary>
        [HttpPost("{id}/upload-image")]
        [ProducesResponseType(typeof(ApiResult<SalonResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResult<object>(false, "Vui lòng chọn ảnh để tải lên."));
            }

            var salonResult = await _salonService.GetSalonByIdAsync(id);
            if (!salonResult.IsSucceeded) return NotFound(salonResult);

            try
            {
                // Tải ảnh lên Cloudinary
                var imageUrl = await _cloudinaryService.UploadImageAsync(file);

                // Cập nhật URL ảnh vào Salon
                var patchRequest = new SalonPatchRequest();
                var response = await _salonService.PatchSalonAsync(id, patchRequest, imageUrl);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResult<object>(false, $"Tải ảnh thất bại: {ex.Message}"));
            }
        }
    }
}